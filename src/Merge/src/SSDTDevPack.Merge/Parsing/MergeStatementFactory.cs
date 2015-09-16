﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using SSDTDevPack.Common.Dac;
using SSDTDevPack.Merge.MergeDescriptor;

namespace SSDTDevPack.Merge.Parsing
{
    public class MergeStatementFactory
    {
        public MergeDescriptor.Merge Build(TableDescriptor table, string scriptFile)
        {
            
            var merge = new MergeDescriptor.Merge();
            merge.Name = table.Name.ToIdentifier();

            merge.Data = BuildDataTableDefinition( table);
            merge.ScriptDescriptor = new InScriptDescriptor(0,0, scriptFile);
            merge.Statement = new MergeStatement();
            merge.Table = table;

            var includeIdentityColumns = false;
            foreach (DataRow row in merge.Data.Rows)
            {
                if (merge.Table.Columns.FirstOrDefault(p => p.IsIdentity) != null &&
                    merge.Table.Columns.Where(p => p.IsIdentity).Any(col => row[col.Name.GetName()] != null))
                {
                    includeIdentityColumns = true;
                }
            }

            merge.Data.AcceptChanges();

            merge.Option = new MergeOptions(true, true, true, includeIdentityColumns);
            return merge;
        }

        private DataTable BuildDataTableDefinition(TableDescriptor table)
        {
            var dataTable = new DataTable();
            foreach (var col in table.Columns)
            {
                dataTable.Columns.Add(new DataColumn(col.Name.GetName()));
            }

            return dataTable;
        }
    }
}
