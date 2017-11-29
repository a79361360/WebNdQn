using Common;
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
        /// 取得T_TopicBank表ID
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
        /// <summary>
        /// 随机取得设置条数的题目
        /// </summary>
        /// <param name="cooperid"></param>
        /// <param name="tmts"></param>
        /// <returns></returns>
        public IList<T_TopicBank> GetDttsTopic(int cooperid,int tmts) {
            IList<T_TopicBank> list = DataTableToList.ModelConvertHelper<T_TopicBank>.ConvertToModel(zdal.GetDttsTopic(cooperid, tmts));
            return list;
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
        public int SetZxdtConfig(int configid, int cooperid, string title, int share, string explain, string bgurl, string wxtitle, string wxdescride, string wximgurl, string wxlinkurl,int tmfs, int tmts, int sright, int flowamount, IList<T_ZxdtScore> list) {
            int result = 0; int resultnum = 0;
            if (string.IsNullOrEmpty(bgurl)) bgurl = "";
            //新增
            if (configid == 0)
            {
                int result_1 = adal.IsExistActivity(cooperid, 2);
                if (result_1 > 0) return -2;    //已经存在当前配置,不能再添加了
                configid = adal.AddConfig(cooperid, 2, title, share, explain, bgurl, wxtitle, wxdescride, wximgurl, wxlinkurl, tmfs, tmts, sright, flowamount); //主表ID
                result = configid;
            }
            else
                result = adal.UpdateConfig(configid, cooperid, 2, title, share, explain, bgurl, wxtitle, wxdescride, wximgurl, wxlinkurl, tmfs, tmts, sright, flowamount);
            if (result < 1) return result;    //如果异常就直接返回
            foreach (var item in list)
            {
                result = zdal.SetZxdtScore(item.id, configid, item.number, item.lower, item.upper);
                if (result > 0)
                    resultnum++;
            }

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
        /// <summary>
        /// 取得在线答题流量设置
        /// </summary>
        /// <param name="configid"></param>
        /// <returns></returns>
        public IList<T_ZxdtScore> GetZxdtScore(int configid) {
            IList<T_ZxdtScore> list = DataTableToList.ModelConvertHelper<T_ZxdtScore>.ConvertToModel(zdal.GetZxdtScore(configid));
            return list;
        }
        /// <summary>
        /// 提交答题信息
        /// </summary>
        /// <param name="cooperid"></param>
        /// <param name="openid"></param>
        /// <param name="phone">手机号码</param>
        /// <param name="score">答题分数</param>
        /// <returns></returns>
        public int SubmitZxdt(int cooperid, string openid, string phone,int score) {
            int result = adal.HandleActivity(1, cooperid, 2, openid, phone, score);
            return result;
        }
        /// <summary>
        /// 取得用户在线答题的列表
        /// </summary>
        /// <param name="cooperid"></param>
        /// <param name="type"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public IList<T_ActivityDrawLog> GetDrawList(int cooperid, int type, string openid)
        {
            IList<T_ActivityDrawLog> list = DataTableToList.ModelConvertHelper<T_ActivityDrawLog>.ConvertToModel(zdal.ZxdtDrawList(cooperid, type, openid));
            return list;
        }
        /// <summary>
        /// 后台查询在线答题记录
        /// </summary>
        /// <param name="cooperid"></param>
        /// <param name="phone"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public IList<T_ActivityDrawLog> ZxdtDrawList_Search(int cooperid, string phone, int state)
        {
            string filter = " WHERE a.type=2 AND (a.configlistid>c.lower AND a.configlistid<=c.upper)";
            if (cooperid != -1)
                filter += " and a.cooperid=@cooperid";
            if (!string.IsNullOrEmpty(phone))
                filter += " and a.phone=@phone";
            if (state != -1)
                filter += " and a.state=@state";
            IList<T_ActivityDrawLog> list = DataTableToList.ModelConvertHelper<T_ActivityDrawLog>.ConvertToModel(zdal.ZxdtDrawList_Search(filter, cooperid, phone, state));
            return list;
        }

        /// <summary>
        /// 取得在线用户已经参与人数的流量值
        /// </summary>
        /// <returns></returns>
        public int ZxdtDrawNumber(int cooperid) {
            int result = zdal.ZxdtDrawNumber(cooperid);
            return result;
        }
    }
}
