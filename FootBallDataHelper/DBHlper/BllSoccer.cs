using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootBallDataHelper.DBHlper
{
    internal class BllSoccer
    {
        //增
        public static int Insert(Model.init_data data)
        {
            data.create_data_time = DateTime.Now.ToString("yyyyMMdd");
            data.update_time=DateTime.Now.ToString("yyyyMMddHHmmss");
            return DataPool.DbSession.Insert(data);
        }
        //改
        public static bool Update(Model.init_data data)
        {
            data.update_time = DateTime.Now.ToString("yyyyMMddHHmmss");
            return DataPool.DbSession.Update<Model.init_data>(data, (x => x.id == data.id)) > 0;
        }
        //选
        public static Model.init_data Select(int detail_id)
        {
            try
            {
                return DataPool.DbSession.From<Model.init_data>().Where(m => m.detail_id == detail_id).First();
            }
            catch
            {
                return null;
            }
        }


    }
}
