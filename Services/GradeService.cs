﻿using System;
using System.Collections.Generic;
using System.Text;
using Xmu.Crms.Services.ViceVersa.Daos;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Services.ViceVersa.Services
{
    class GradeService : IGradeService
    {
        private readonly IGradeDao _iGradeDao;
        private readonly IUserService _iUserService;
        private readonly ITopicService _iTopicService;
        private readonly ISeminarGroupService _iSeminarGroupService;
        private readonly ISeminarService _iSeminarService;

        public GradeService(IGradeDao iGradeDao, IUserService iUserService, ITopicService iTopicService, ISeminarGroupService iSeminarGroupService, ISeminarService iSeminarService)
        {
            _iGradeDao = iGradeDao;
            _iUserService = iUserService;
            _iTopicService = iTopicService;
            _iSeminarGroupService = iSeminarGroupService;
            _iSeminarService = iSeminarService;
        }

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

        public SeminarGroup GetSeminarGroupBySeminarGroupId(long userId, long seminarGroupId)
        {
            try//此处userId无用，将改
            {
                return _iGradeDao.GetSeminarGroupBySeminarGroupId(seminarGroupId);
            }
            catch
            {
                throw;
            }
        }

        public IList<SeminarGroup> ListSeminarGradeByCourseId(long userId, long courseId)
        {
            List<SeminarGroup> seminarGroupList = new List<SeminarGroup>();
            List<Seminar> seminarList = new List<Seminar>();
            try
            {
                //调用SeminarService 中 IList<Seminar> ListSeminarByCourseId(long courseId)方法
                _iSeminarService.ListSeminarByCourseId(courseId);

                //调用SeminarGroupService 中 SeminarGroup GetSeminarGroupById(long seminarId, long userId)
                for (int i = 0; i < seminarList.Count; i++)
                {
                    seminarGroupList.Add(_iSeminarGroupService.GetSeminarGroupById(seminarList[0].Id, userId));
                }
            }
            catch
            {
                throw;
            }
            return seminarGroupList;
        }

        public void InsertGroupGradeByUserId(long topicId, long userId,long groupId, int grade)
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
            catch
            {
                throw;
            }
        }

        public void UpdateGroupByGroupId(long seminarGroupId, int grade)
        {
            try
            {
                _iGradeDao.UpdateGroupByGroupId(seminarGroupId, grade);
            }
            catch
            {
                throw;
            }
        }

        public IList<SeminarGroup> ListSeminarGradeByStudentId(long userId)
        {
            try
            {
                return _iSeminarGroupService.ListSeminarGroupIdByStudentId(userId);
            }
            catch
            {
                throw;
            }

        }

        public void CountGroupGradeBySerminarId(long seminarId, long seminarGroupId)
        {
            _iGradeDao.CountGroupGradeBySerminarId(seminarId,seminarGroupId);
        }

        public void CountPresentationGrade(long seminarId, long seminarGroupId)
        { 
            //调用TopicService 的 IList<Topic> ListTopicBySeminarId(long seminarId)方法
            IList<Topic> topicList = _iTopicService.ListTopicBySeminarId(seminarId);

            //调用自己的dao分别对每个topic计算
            _iGradeDao.CountPresentationGrade(seminarId, topicList);
        }

    }
}
