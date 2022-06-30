using Dos.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootBallDataHelper.DBHlper
{
    internal class DataPool
    {
        public static readonly DbSession DbSession = new DbSession(DatabaseType.Sqlite3, "Data Source=matchdata;Version=3;");
    }
}
