using Luban.RawDefs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Schema;

public interface ITableImporter
{
    List<RawTable> LoadImportTables();
}
