using System;
using System.Collections.Generic;
using System.Text;
using Xmu.Crms.Shared.Models;

namespace Xmu.Crms.Services.ViceVersa.Daos
{
    /// <summary>
    /// 成绩处理模块Dao层实现
    /// @author Group ViceVersa
    /// </summary>
    public interface IGradeDao
    {
        /// <summary>
        /// 按topicId删除学生打分表.
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="topicId">话题Id</param>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.GroupNotFoundException">未找到小组</exception>
        void DeleteStudentScoreGroupByTopicId(long topicId);

        ///<summary>
        ///获取某学生一堂讨论课信息
        ///@author Group ViceVersa
        ///获取某学生一堂讨论课的详细信息（包括成绩）
        /// </summary>
        /// <param name="seminarGroupId">讨论课小组Id</param>
        /// <returns>seminarGroup 讨论课小组信息（包括成绩）</returns>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.GroupNotFoundException">未找到小组</exception>
        SeminarGroup GetSeminarGroupBySeminarGroupId(long seminarGroupId);

        /// <summary>
        /// 不用写，调用其他Dao的方法
        /// </summary>
        List<SeminarGroup> ListSeminarGradeByCourseId(long userId, long courseId);

        /// <summary>
        /// 提交对其他小组的打分.
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="seminarGroupTopic"></param>
        /// <param name="userInfo"></param>
        /// <param name="grade"></param>
        void InsertGroupGradeByUserId(SeminarGroupTopic seminarGroupTopic, UserInfo userInfo, int grade);

        /// <summary>
        /// 按ID设置小组报告分.
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="seminarGroupId">讨论课组id</param>
        /// <param name="grade">分数</param>
        /// <exception cref="T:System.GroupNotFoundException">该小组未找到</exception>
        void UpdateGroupByGroupId(long seminarGroupId, int grade);

        /// <summary>
        /// 不用写，在SeminarGroupService调用 IList<SeminarGroup> ListSeminarGroupIdByStudentId(long userId);
        /// </summary>
        List<SeminarGroup> ListSeminarGradeByStudentId(long userId);

        /// <summary>
        /// 根据讨论课Id和话题计算展示成绩，排序打分
        /// </summary>
        /// <param name="seminarId"></param>
        /// <param name="topicList"></param>
        void CountPresentationGrade(long seminarId, IList<Topic> topicList);

        /// <summary>
        /// 定时器方法:讨论课结束后计算本次讨论课得分.
        /// @author Group ViceVersa
        /// 条件: 讨论课已结束，展示得分已算出  *GradeService
        /// </summary>
        /// <param name="seminarId">讨论课id</param>
        /// <param name="seminarGroupList">讨论课小组序列</param>
        /// <exception cref="T:System.GroupNotFoundException">讨论课小组未找到错误</exception>
        void CountGroupGradeBySerminarId(long seminarId, IList<SeminarGroup> seminarGroupList);
    }
}
