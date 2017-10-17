﻿using Common;
using DAL;
using Model.WxModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.ViewModel;

namespace BLL
{
    public class ZxdtBLL
    {
        ZxdtDAL zdal = new ZxdtDAL();
        ActivityDAL adal = new ActivityDAL();
        #region 题库部分
        /// <summary>
        /// 取得T_TopicBank用户表ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T_TopicBank GetTopicById(int id) {
            T_TopicBank dto = new T_TopicBank();
            IList<T_TopicBank> list = DataTableToList.ModelConvertHelper<T_TopicBank>.ConvertToModel(zdal.GetTopicById(id));
            if (list.Count > 0) 
                dto = list[0];
            return dto;
        }
        /// <summary>
        /// 读取题库列表,翻页
        /// </summary>
        /// <param name="cooperid"></param>
        /// <param name="title"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="Total"></param>
        /// <returns></returns>
        public IList<T_TopicBank> FindTopicList_Page(int cooperid, string title, int pageSize, int pageIndex, ref int Total)
        {
            string filter = "";
            if (cooperid != -1)
                filter += " cooperid=" + cooperid;
            if (!string.IsNullOrEmpty(title))
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " and topic like '%" + title + "%'";
                else
                    filter += " topic like '%" + title + "%'";
            }
            SqlPageParam param = new SqlPageParam();
            param.TableName = "T_TopicBank";
            param.PrimaryKey = "id";
            param.Fields = "[id],[cooperid],[topic],[answer],[keyanswer],[addtime]";
            param.PageSize = pageSize;
            param.PageIndex = pageIndex;
            param.Filter = filter;
            param.Group = "";
            param.Order = "id";
            IList<T_TopicBank> list = DataTableToList.ModelConvertHelper<T_TopicBank>.ConvertToModel(zdal.PageResult(ref Total, param));
            return list;
        }
        /// <summary>
        /// 设置题库,如果ID为0为新增,非0为更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cooperid"></param>
        /// <param name="topic"></param>
        /// <param name="answer"></param>
        /// <param name="keyanswer"></param>
        /// <returns></returns>
        public int SetZxdtTopic(int id,int cooperid,string topic,string answer,int keyanswer) {
            int result = 0;
            T_TopicBank dto = new T_TopicBank();
            dto.id = id;dto.cooperid = cooperid;dto.topic = topic;dto.answer = answer;dto.keyanswer = keyanswer;
            result = zdal.SetZxdtTopic(dto);
            return result;
        }
        /// <summary>
        /// 删除题库
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public int RemoveTopics(IList<IdListDto> ids)
        {
            int sresult = 0;    //成功的数量
            if (ids.Count == 0)
                throw new ArgumentNullException();
            else
            {
                try
                {
                    foreach (var item in ids)
                    {
                        int gid = item.id;        //ID
                        int result = zdal.TopicRemoveById(gid);
                        sresult = sresult + result;
                    }
                }
                catch
                {
                    return -1000;
                }
            }
            return sresult;
        }
        #endregion
        /// <summary>
        /// 取得活动主表配置信息
        /// </summary>
        /// <param name="cooper"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public T_ActivityConfig GetByCooperId(int cooperid,int type) {
            IList<T_ActivityConfig> list = DataTableToList.ModelConvertHelper<T_ActivityConfig>.ConvertToModel(adal.GetActivityZb(cooperid, type));
            T_ActivityConfig dto = new T_ActivityConfig();
            if (list.Count > 0)
                dto = list[0];
            return dto;
        }
        /// <summary>
        /// 设置在线答题配置
        /// </summary>
        /// <param name="configid"></param>
        /// <param name="cooperid"></param>
        /// <param name="title"></param>
        /// <param name="share"></param>
        /// <param name="explain"></param>
        /// <param name="bgurl"></param>
        /// <param name="wxtitle"></param>
        /// <param name="wxdescride"></param>
        /// <param name="wximgurl"></param>
        /// <param name="wxlinkurl"></param>
        /// <returns></returns>
        public int SetZxdtConfig(int configid, int cooperid, string title, int share, string explain, string bgurl, string wxtitle, string wxdescride, string wximgurl, string wxlinkurl) {
            int result = 0;
            if (string.IsNullOrEmpty(bgurl)) { bgurl = "/Content/Activity/Dzp/images/body_bg1.jpg"; }
            //新增
            if (configid == 0)
            {
                int result_1 = adal.IsExistActivity(cooperid, 2);
                if (result_1 > 0) return -2;    //已经存在当前配置,不能再添加了
                configid = adal.AddConfig(cooperid, 2, title, share, explain, bgurl, wxtitle, wxdescride, wximgurl, wxlinkurl); //主表ID
                result = configid;
            }
            else
                result = adal.UpdateConfig(configid, cooperid, 2, title, share, explain, bgurl, wxtitle, wxdescride, wximgurl, wxlinkurl);
            return result;
        }
        /// <summary>
        /// 查询活动主表配置列表
        /// </summary>
        /// <param name="type">1大转盘,2在线答题</param>
        /// <param name="name">条件name</param>
        /// <param name="value">条件value</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="Total"></param>
        /// <returns></returns>
        public IList<T_ActivityConfig> GetActivity_Page(int type, string name, string value, int pageSize, int pageIndex, ref int Total)
        {
            string filter = "";
            if (name != "-1")
            {
                filter += name + " like '%" + value + "%'";
            }
            if (type > 0)
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " and type=" + type;
                else
                    filter += " type=" + type;
            }
            SqlPageParam param = new SqlPageParam();
            param.TableName = "V_ActivityConfig";
            param.PrimaryKey = "id";
            param.Fields = "id,cooperid,ctype,type,title,share,explain,bgurl";
            param.PageSize = pageSize;
            param.PageIndex = pageIndex;
            param.Filter = filter;
            param.Group = "";
            param.Order = "id";
            IList<T_ActivityConfig> list = DataTableToList.ModelConvertHelper<T_ActivityConfig>.ConvertToModel(adal.PageResult(ref Total, param));
            return list;
        }

    }
}
