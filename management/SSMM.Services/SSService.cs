﻿using SSMM.Cache;
using SSMM.Entity;
using SSMM.Helper;
using SSMM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMM.Services
{
    public class SSService
    {
        public static object obj = new object();
        /// <summary>
        /// 获取ss端口
        /// </summary>
        public static int GetNextPort()
        {
            lock (obj)
            {
                var port = 0;
                var range = SettingCache.Cache.Get(SettingFlag.SSPortRange);
                if (range == null)
                    throw new Exception("请先后台配置ss连接端口范围");
                var min = Convert.ToInt32(range.Split('&')[0]);
                var max = Convert.ToInt32(range.Split('&')[1]);
                using (var DB = new SSMMEntities())
                {
                    var current = DB.SS.OrderByDescending(x => x.port).SingleOrDefault();
                    if (current == null)
                        port = min;
                    else
                        port = current.port + 1;
                }
                if (port > max)
                    throw new Exception("端口已超过最大范围");
                return port;
            }
        }


        public static SSDto Query(string id)
        {
            using (var DB = new SSMMEntities())
            {
                var ss = DB.SS.Find(id);
                if (ss == null)
                    return null;

                var now = FormatHelper.ConvertDateTimeInt(DateTime.Now);
                return new SSDto()
                {
                    id = ss.id,
                    t = ss.t,
                    u = ss.u,
                    d = ss.d,
                    transfer_enable = ss.transfer_enable,
                    port = ss.port,
                    password = ss.password,
                    @switch = ss.@switch,
                    enable = ss.enable,
                    isrest = ss.isrest,
                    last_rest_time = ss.last_rest_time,
                    expiration_time = ss.expiration_time,
                    create_time = ss.create_time,
                    userid = ss.userid,
                    status = (ss.u + ss.d) < ss.transfer_enable && ss.@switch == 1 && ss.enable == 1 && ss.expiration_time > now
                };
            }
        }

        public static SSDto Query(int userid)
        {
            using (var DB = new SSMMEntities())
            {
                var ss = DB.SS.SingleOrDefault(x => x.userid == userid);
                if (ss == null)
                    return null;
                var now = FormatHelper.ConvertDateTimeInt(DateTime.Now);
                return new SSDto()
                {
                    id = ss.id,
                    t = ss.t,
                    u = ss.u,
                    d = ss.d,
                    transfer_enable = ss.transfer_enable,
                    port = ss.port,
                    password = ss.password,
                    @switch = ss.@switch,
                    enable = ss.enable,
                    isrest = ss.isrest,
                    last_rest_time = ss.last_rest_time,
                    expiration_time = ss.expiration_time,
                    create_time = ss.create_time,
                    userid = ss.userid,
                    status = (ss.u + ss.d) < ss.transfer_enable && ss.@switch == 1 && ss.enable == 1 && ss.expiration_time > now
                };
            }
        }

        /// <summary>
        /// 列表
        /// </summary>
        public static List<SSDto> GetList(int offset, int limit, out int totalcount, string key = null)
        {
            var models = new List<SSDto>();
            using (var DB = new SSMMEntities())
            {
                var list = DB.SS.Where(x => true);
                if (!string.IsNullOrEmpty(key))
                    list = list.Where(x => x.id.Contains(key) || x.password.Contains(key));
                totalcount = list.Count();
                var result = list.OrderByDescending(x => x.create_time)
                                  .Skip(offset)
                                  .Take(limit)
                                  .ToList();

                var now = FormatHelper.ConvertDateTimeInt(DateTime.Now);
                result.ForEach(x =>
                {
                    models.Add(new SSDto()
                    {
                        id = x.id,
                        t = x.t,
                        u = x.u,
                        d = x.d,
                        transfer_enable = x.transfer_enable,
                        port = x.port,
                        password = x.password,
                        @switch = x.@switch,
                        enable = x.enable,
                        isrest = x.isrest,
                        last_rest_time = x.last_rest_time,
                        expiration_time = x.expiration_time,
                        create_time = x.create_time,
                        userid = x.userid,
                        status = (x.u + x.d) < x.transfer_enable && x.@switch == 1 && x.enable == 1 && x.expiration_time > now
                    });
                });
            }
            return models;
        }

        /// <summary>
        /// 添加
        /// </summary>
        public static bool Add(SSDto dto)
        {
            using (var DB = new SSMMEntities())
            {
                DB.SS.Add(new SS()
                {
                    id = dto.id,
                    t = dto.t,
                    u = dto.u,
                    d = dto.d,
                    transfer_enable = dto.transfer_enable,
                    port = dto.port,
                    password = dto.password,
                    @switch = dto.@switch,
                    enable = dto.enable,
                    isrest = dto.isrest,
                    last_rest_time = dto.last_rest_time,
                    expiration_time = dto.expiration_time,
                    create_time = dto.create_time,
                    userid = dto.userid
                });
                return DB.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public static bool Update(SSDto dto)
        {
            using (var DB = new SSMMEntities())
            {
                var model = DB.SS.Find(dto.id);
                if (model != null)
                {
                    model.id = dto.id;
                    model.t = dto.t;
                    model.u = dto.u;
                    model.d = dto.d;
                    model.transfer_enable = dto.transfer_enable;
                    model.port = dto.port;
                    model.password = dto.password;
                    model.@switch = dto.@switch;
                    model.enable = dto.enable;
                    model.isrest = dto.isrest;
                    model.last_rest_time = dto.last_rest_time;
                    model.expiration_time = dto.expiration_time;
                    model.create_time = dto.create_time;
                    model.userid = dto.userid;
                }
                return DB.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public static bool Delete(string id)
        {
            using (var DB = new SSMMEntities())
            {
                var model = DB.SS.Find(id);
                if (model != null)
                {
                    DB.SS.Remove(model);
                }
                return DB.SaveChanges() > 0;
            }
        }



    }
}
