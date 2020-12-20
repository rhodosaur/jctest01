﻿using Jerrycurl.Relations;
using Jerrycurl.Test.Models.Database;

namespace Jerrycurl.Data.Test.Models.Views
{
    internal class BlogTagView : BlogTagMap
    {
        public One<BlogTag> Tag { get; set; }
        public One<BlogPost> Post { get; set; }
    }
}
