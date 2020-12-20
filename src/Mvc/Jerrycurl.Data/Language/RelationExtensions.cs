﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Jerrycurl.Data.Commands;
using Jerrycurl.Data.Queries;
using Jerrycurl.Data.Sessions;
using Jerrycurl.Relations;
using Jerrycurl.Relations.Language;
using Jerrycurl.Relations.Metadata;

namespace Jerrycurl.Data.Language
{
    public static class RelationExtensions
    {
        public static ISchema Describe<T>(this ISchemaStore store)
            => store.GetSchema(typeof(T));

        public static QueryBuffer AsBuffer(this ISchema schema, QueryType type)
            => new QueryBuffer(schema, type);

        public static Query AsQuery(this IRelation relation, string queryText)
        {
            return new Query()
            {
                QueryText = queryText,
                Parameters = relation.AsParameters()
            };
        }

        public static Command AsCommand(this IRelation relation, string commandText)
        {
            return new Command()
            {
                CommandText = commandText,
                Parameters = relation.AsParameters()
            };
        }

        public static Command AsCommand(this IRelation relation, Func<IList<IParameter>, string> textBuilder)
        {
            ParameterStore store = new ParameterStore();

            Command command = new Command()
            {
                Parameters = store,
            };

            using var reader = relation.GetReader();

            while (reader.Read())
            {
                IList<IParameter> parameters = store.Add(reader);

                command.CommandText += textBuilder(parameters) + Environment.NewLine;
            }

            return command;
        }

        public static DbDataReader As(this IRelation relation, IEnumerable<string> header)
            => relation.GetDataReader(header);

        public static DbDataReader As(this IRelation relation, params string[] header)
            => relation.GetDataReader(header);

        public static IList<IParameter> AsParameters(this IField source, params string[] header)
            => source.Select(header).AsParameters();

        public static IList<IParameter> AsParameters(this IField source, IEnumerable<string> header)
            => source.Select(header).AsParameters();

        public static IList<IParameter> AsParameters(this ITuple tuple)
            => new ParameterStore().Add(tuple);

        public static IList<IParameter> AsParameters(this IRelation relation)
            => new ParameterStore().Add(relation);

        public static IParameter AsParameter(this IField field, string parameterName)
            => new Parameter(parameterName, field);
    }
}