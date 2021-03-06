﻿using Jerrycurl.Data.Queries.Internal;
using Jerrycurl.Data.Queries.Internal.State;
using Jerrycurl.Relations.Metadata;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Jerrycurl.Data.Queries
{
    public class QueryReader
    {
        private readonly IDataReader syncReader;
        private readonly DbDataReader asyncReader;
        private readonly ISchemaStore schemas;

        internal QueryReader(IDataReader dataReader, ISchemaStore schemas)
        {
            this.syncReader = dataReader ?? throw new ArgumentNullException(nameof(dataReader));
            this.schemas = schemas ?? throw new ArgumentNullException(nameof(schemas));
        }

        internal QueryReader(DbDataReader dataReader, ISchemaStore schemas)
        {
            this.syncReader = this.asyncReader = dataReader ?? throw new ArgumentNullException(nameof(dataReader));
            this.schemas = schemas ?? throw new ArgumentNullException(nameof(schemas));
        }

        public async IAsyncEnumerable<TItem> ReadAsync<TItem>([EnumeratorCancellation]CancellationToken cancellationToken = default)
        {
            if (this.asyncReader == null)
                throw new QueryException("Async not available. To use async operations, please supply a connection factory returning a DbConnection instance.");

            TableIdentity heading = TableIdentity.FromRecord(this.asyncReader);
            ResultState<TItem> state = ResultCache<TItem>.GetResultState(this.schemas, heading);

            while (await this.asyncReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                yield return state.Item(this.asyncReader);
        }

        public IEnumerable<TItem> Read<TItem>()
        {
            TableIdentity heading = TableIdentity.FromRecord(this.syncReader);
            ResultState<TItem> state = ResultCache<TItem>.GetResultState(this.schemas, heading);

            while (this.syncReader.Read())
                yield return state.Item(this.syncReader);
        }
    }
}