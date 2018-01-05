using System;
using System.Collections.Generic;
using System.Text;
using Xmu.Crms.Services.ViceVersa.Daos;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Services.ViceVersa.Services
{
    /// <summary>
    /// 成绩处理模块逻辑层实现
    /// @author Group ViceVersa
    /// </summary>
    class GradeService : IGradeService
    {
        private readonly IGradeDao _iGradeDao;
        private readonly IUserService _iUserService;
        private readonly ITopicService _iTopicService;
        private readonly ISeminarGroupService _iSeminarGroupService;
        private readonly ISeminarService _iSeminarService;

        /// <summary>
        /// 将所需用到的Dao注入该Service
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="iGradeDao"></param>
        /// <param name="iUserService"></param>
        /// <param name="iTopicService"></param>
        /// <param name="iSeminarGroupService"></param>
        /// <param name="iSeminarService"></param>
        public GradeService(IGradeDao iGradeDao, IUserService iUserService, ITopicService iTopicService, ISeminarGroupService iSeminarGroupService, ISeminarService iSeminarService)
        {
            _iGradeDao = iGradeDao;
            _iUserService = iUserService;
            _iTopicService = iTopicService;
            _iSeminarGroupService = iSeminarGroupService;
            _iSeminarService = iSeminarService;
        }

        /// <summary>
        /// 按topicId删除学生打分表.
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="topicId">话题Id</param>
        public void DeleteStudentScoreGroupByTopicId(long topicId)
        {
            try
            {
                _iGradeDao.DeleteStudentScoreGroupByTopicId(topicId);
            }
            catch
            {
                throw;
            }
        }

        ///<summary>
        ///获取某学生一堂讨论课信息
        ///@author Group ViceVersa
        ///获取某学生一堂讨论课的详细信息（包括成绩）
        /// </summary>
        /// <param name="seminarGroupId">讨论课小组Id</param>
        /// <returns>seminarGroup 讨论课小组信息（包括成绩）</returns>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.GroupNotFoundException">未找到小组</exception>
        public SeminarGroup GetSeminarGroupBySeminarGroupId(long seminarGroupId)
        {
            try
            {
                return _iGradeDao.GetSeminarGroupBySeminarGroupId(seminarGroupId);
            }
            catch(GroupNotFoundException e1)
            {
                throw e1;
            }
        }

        /// <summary>
        /// 按课程id获取学生该课程所有讨论课
        /// @author Group ViceVersa
        /// 通过课程id获取该课程下学生所有讨论课详细信息（包括成绩）
        /// </summary>
        /// <param name="userId">学生id</param>
        /// <param name="courseId">课程id</param>
        /// <returns>list 该课程下所有讨论课列表</returns>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ISeminarService.ListSeminarByCourseId(System.Int64)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ISeminarGroupService.ListSeminarGroupBySeminarId(System.Int64)"/>
        public IList<SeminarGroup> ListSeminarGradeByCourseId(long userId, long courseId)
        {
            List<SeminarGroup> seminarGroupList = new List<SeminarGroup>();
            
            try
            {
                //调用SeminarService 中 IList<Seminar> ListSeminarByCourseId(long courseId)方法
                IList<Seminar> seminarList = _iSeminarService.ListSeminarByCourseId(courseId);

                //调用SeminarGroupService 中 SeminarGroup GetSeminarGroupById(long seminarId, long userId)
                for (int i = 0; i < seminarList.Count; i++)
                {
                    SeminarGroup seminarGroup = _iSeminarGroupService.GetSeminarGroupById(seminarList[i].Id, userId);
                    if(seminarGroup!=null)
                       seminarGroupList.Add(seminarGroup);
                }
            }
            catch(CourseNotFoundException cre)
            {
                throw cre;
            }
            catch (Exception e)
            {
                throw e;
            }
            return seminarGroupList;
        }

        /// <summary>
        /// 提交对其他小组的打分.
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="topicId">话题Id</param>
        /// <param name="seminarId">讨论课Id</param>
        /// <param name="groupId">小组Id</param>
        /// <param name="grade">分数</param>
        /// <exception cref="T:System.GroupNotFoundException">该小组未找到</exception>
        /// <exception cref="T:System.UserNotFoundException">该用户未找到</exception>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ITopicService.GetSeminarGroupTopicById(System.Int64)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IUserService.GetUserByUserId(System.Int64)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IGradeService.InsertGroupGradeByUserId(System.Int64)"/>
        public void InsertGroupGradeByUserId(long topicId, long userId, long groupId, int grade)
        {
            try
            {
                //调用TopicService中GetSeminarGroupTopicById(long topicId, long groupId)方法 
                SeminarGroupTopic seminarGroupTopic = _iTopicService.GetSeminarGroupTopicById(topicId, groupId);
                //调用UserService中的GetUserByUserId(long userId)方法
                UserInfo userInfo = _iUserService.GetUserByUserId(userId);
                //调用自己的dao
                _iGradeDao.InsertGroupGradeByUserId(seminarGroupTopic, userInfo, grade);
            }
            catch(GroupNotFoundException gre)
            {
                throw gre;
            }
            catch(UserNotFoundException ure)
            {
                throw ure;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 按ID设置小组报告分.
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="seminarGroupId">讨论课组id</param>
        /// <param name="grade">分数</param>
        /// <exception cref="T:System.GroupNotFoundException">该小组未找到</exception>
        public void UpdateGroupByGroupId(long seminarGroupId, int grade)
        {
            try
            {
                _iGradeDao.UpdateGroupByGroupId(seminarGroupId, grade);
            }
            catch (GroupNotFoundException gre)
            {
                throw gre;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 获取某学生的讨论课成绩列表.
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <exception cref="T:System.UserNotFoundException">该用户未找到</exception>
        public IList<SeminarGroup> ListSeminarGradeByStudentId(long userId)
        {
            try
            {
                return _iSeminarGroupService.ListSeminarGroupIdByStudentId(userId);
            }
            catch (UserNotFoundException ure)
            {
                throw ure;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 定时器方法:讨论课结束后计算展示得分.
        /// @author Group ViceVersa
        /// 条件: 讨论课已结束  *GradeService
        /// </summary>
        /// <param name="seminarId">讨论课id</param>
        /// <exception cref="T:System.TopicNotFoundException">话题未找到错误</exception>
        /// <exception cref="T:System.GroupNotFoundException">小组未找到错误</exception>
        /// <exception cref="T:System.SeminarNotFoundException">讨论课未找到错误</exception>
        /// <exception cref="T:System.ClassNotFoundException">班级未找到错误</exception>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ITopicService.ListTopicBySeminarId(System.Int64)"/>
        public void CountPresentationGrade(long seminarId)
        {
            try
            {
                //调用TopicService 的 IList<Topic> ListTopicBySeminarId(long seminarId)方法
                IList<Topic> topicList = _iTopicService.ListTopicBySeminarId(seminarId);
           
                //调用自己的dao分别对每个topic计算
                _iGradeDao.CountPresentationGrade(seminarId, topicList);
            }
            catch (TopicNotFoundException ure)
            {
                throw ure;
            }
            catch (GroupNotFoundException gre)
            {
                throw gre;
            }
            catch (SeminarNotFoundException sme)
            {
                throw sme;
            }
            catch(ClassNotFoundException cle)
            {
                throw cle;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 定时器方法:讨论课结束后计算本次讨论课得分.
        /// @author Group ViceVersa
        /// 条件: 讨论课已结束，展示得分已算出  *GradeService
        /// </summary>
        /// <param name="seminarId">讨论课id</param>
        /// <exception cref="T:System.SeminarNotFoundException">讨论课未找到错误</exception>
        /// <exception cref="T:System.GroupNotFoundException">讨论课小组未找到错误</exception>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ISeminarService.ListSeminarGroupBySeminarId(System.Int64)"/>
        public void CountGroupGradeBySerminarId(long seminarId)
        {
            try
            {
                ////调用SeminarGroupService中的ListSeminarGroupBySeminarId 方法
                IList<SeminarGroup> seminarGroupList = _iSeminarGroupService.ListSeminarGroupBySeminarId(seminarId);

                _iGradeDao.CountGroupGradeBySerminarId(seminarId, seminarGroupList);
            }
            catch (SeminarNotFoundException sme)
            {
                throw sme;
            }
            catch (GroupNotFoundException gre)
            {
                throw gre;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
