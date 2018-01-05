using System;
using System.Collections.Generic;
using System.Transactions;
using Xmu.Crms.Services.ViceVersa.Daos;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Services.ViceVersa
{
    /// <summary>
    /// @author Group ViceVersa
    /// @version 2.00
    /// </summary>
    class ClassService : IClassService
    {
        private readonly ISeminarService _seminarService;
        private readonly IUserService _userService;
        private readonly IFixGroupService _fixGroupService;
        private readonly IClassDao _classDao;

        /// <summary>
        /// 构造函数 services注入
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="seminarService">seminarService</param>
        /// <param name="classDao">classDao</param>
        /// <param name="userService">userService</param>
        /// <param name="fixGroupService">fixGroupService</param>
        public ClassService(ISeminarService seminarService, IUserService userService, IFixGroupService fixGroupService, IClassDao classDao)
        {
            _seminarService = seminarService;
            _userService = userService;
            _fixGroupService = fixGroupService;
            _classDao = classDao;
        }

        /// <summary>
        /// 按班级id删除班级.
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="classId">班级ID</param>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IClassService.DeleteScoreRuleById(System.Int64)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IClassService.DeleteCourseSelectionById(System.Int64,System.Int64)"/>
        public void DeleteClassByClassId(long classId)
        {
                try
                {
                _classDao.Delete(classId);
                    
                }
                catch (ClassNotFoundException ec)
                {
                    
                    throw ec;
                }
            
        }


        /// <summary>
        /// 按courseId删除Class.
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="courseId">课程Id</param>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IClassService.ListClassByCourseId(System.Int64)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IClassService.DeleteClassSelectionByClassId(System.Int64)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IClassService.DeleteScoreRuleById(System.Int64)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IFixGroupService.DeleteFixGroupByClassId(System.Int64)"/>
        public void DeleteClassByCourseId(long courseId)
        {
            try
            {
                //根据课程id获得所有班级信息
                List<ClassInfo> deleteClasses = _classDao.QueryAll(courseId);
                foreach (ClassInfo c in deleteClasses)
                {
                    _fixGroupService.DeleteFixGroupByClassId(c.Id);
                    // 根据class信息删除courseSelection表的记录 并删除班级
                    _classDao.Delete(c.Id);
                }
            }
            catch (ClassNotFoundException el) {throw el; }
            catch (CourseNotFoundException e) { throw e; }
        }


        /// <summary>
        /// 按classId删除CourseSelection表的一条记录.
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="classId">班级Id</param>
        public void DeleteClassSelectionByClassId(long classId)
        {
            _classDao.DeleteSelection(0, classId);
        }


        /// <summary>
        /// 学生按班级id取消选择班级.
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="classId">班级id</param>
        public void DeleteCourseSelectionById(long userId, long classId)
        {
            try
            {
                //_userService.GetUserByUserId(userId);
                GetClassByClassId(classId);
                _classDao.DeleteSelection(userId, classId);

            }catch(UserNotFoundException eu) { throw eu; }
            catch (ClassNotFoundException ec) { throw ec; }
        }


        /// <summary>
        /// 按班级id删除评分规则.
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="classId">班级id</param>
        public void DeleteScoreRuleById(long classId)
        {
            try
            {
                ClassInfo newclass = new ClassInfo() { Id=classId};
                newclass.ReportPercentage = 0;
                newclass.PresentationPercentage = 0;
                newclass.FivePointPercentage = 0;
                newclass.FourPointPercentage = 0;
                newclass.ThreePointPercentage = 0;
                var result = _classDao.Update(newclass);
                _classDao.Update(newclass);
            }
            catch (ClassNotFoundException e) { throw e; }
        }


        /// <summary>
        /// 老师获取位置信息，获取班级签到状态.
        /// 
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="seminarId">讨论课id</param>
        /// <param name="classId">班级id</param>
        /// <returns>location 班级签到状态</returns>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ISeminarGroupService.ListSeminarGroupBySeminarId(System.Int64)"/>
        public Location GetCallStatusById(long seminarId, long classId)
        {
            try
            {
                _seminarService.GetSeminarBySeminarId(seminarId);
                //_classDao.Get(classId);
                return _classDao.GetLocation(seminarId, classId);
            }
            catch (SeminarNotFoundException e) { throw e; }
        }


        /// <summary>
        /// 按班级id获取班级详情.
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="classId">班级ID</param>
        /// <returns>ClassBO 班级</returns>
         public ClassInfo GetClassByClassId(long classId)
        {

            try
            {
                ClassInfo classinfo = _classDao.Get(classId);
                return classinfo;
            }
            catch (ClassNotFoundException e) { throw e; }
        }


         /// <summary>
         /// 按班级id获取班级评分规则.
         /// @author Group ViceVersa
         /// </summary>
         /// <param name="classId">班级ID</param>
         /// <returns>ClassBO 班级</returns>
        public ClassInfo GetScoreRule(long classId)
        {
            try
            {
                return _classDao.Get(classId);
            }
            catch (ClassNotFoundException e) { throw e; }
        }



        /// <summary>
        /// 学生按班级id选择班级.
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="classId">班级id</param>
        /// <returns>courseSelectionId 选课记录id</returns>
        public long InsertCourseSelectionById(long userId, long classId)
        {
            try
            {
                //_userService.GetUserByUserId(userId);
                ClassInfo classinfo= GetClassByClassId(classId);

                //找到该班级所属课程下的所有班级
                IList<ClassInfo> classList = ListClassByCourseId(classinfo.Course.Id);
                foreach(ClassInfo c in classList)
                {
                    if (_classDao.GetSelection(userId, c.Id) != 0)//学生已选同课程下其他班级
                        return 0;
                }
               CourseSelection coursesele = new CourseSelection();

                
                UserInfo student = _userService.GetUserByUserId(userId);
                coursesele.Student = student;
                coursesele.ClassInfo = classinfo;
                return _classDao.InsertSelection(coursesele);
            }
            catch (UserNotFoundException eu) { throw eu; }
            catch(ClassNotFoundException ec) { throw ec; }
           
        }


        /// <summary>
        /// 按班级id和班级新增评分规则.
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="classId">班级ID</param>
        /// <param name="newclass">修改后班级信息</param>
        /// <returns>ClassID 班级ID</returns>
        public long InsertScoreRule(long classId, ClassInfo proportions)
        {
            try
            {
                if (proportions.ReportPercentage < 0 || proportions.ReportPercentage > 100 ||
                    proportions.PresentationPercentage < 0 || proportions.PresentationPercentage > 100 ||
                    proportions.ReportPercentage + proportions.PresentationPercentage != 100 ||
                    proportions.FivePointPercentage < 0 || proportions.FivePointPercentage > 10 ||
                    proportions.FourPointPercentage < 0 || proportions.FourPointPercentage > 10 ||
                    proportions.ThreePointPercentage < 0 || proportions.ThreePointPercentage > 10 ||
                    proportions.FivePointPercentage + proportions.FourPointPercentage + proportions.ThreePointPercentage != 10)
                    throw new InvalidOperationException();
                var result = _classDao.Update(proportions);//新建班级时已经建了一个空的
                if (result != 0) return -1;
                return classId;
            }
            catch (InvalidOperationException ei) { throw ei; }
            catch (ClassNotFoundException ec) { throw ec; }

        }


        /// <summary>
        /// 根据课程ID获得班级列表.
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="courseId">课程ID</param>
        /// <returns>list 班级列表</returns>
        public IList<ClassInfo> ListClassByCourseId(long courseId)
        {
            try
            {
                List<ClassInfo> list = _classDao.QueryAll(courseId);
                return list;
            }
            catch (CourseNotFoundException e) { throw e; }
        }

        /// <summary>
        /// 按班级id和班级修改班级信息.
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="classId">班级ID</param>
        /// <param name="newclass">修改后班级信息</param>
        public void UpdateClassByClassId(long classId, ClassInfo newclass)
        {
            try
            {
                var result = _classDao.Update(newclass);  //return 0成功更新
            }
            catch (ClassNotFoundException e) { throw e; }
        }


        /// <summary>
        /// 按班级id和班级修改评分规则.
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="classId">班级ID</param>
        /// <param name="newclass">修改后班级信息</param>
        public void UpdateScoreRule(long classId, ClassInfo proportions)
        {
            try
            {
                if (proportions.ReportPercentage < 0 || proportions.ReportPercentage > 100 ||
                    proportions.PresentationPercentage < 0 || proportions.PresentationPercentage > 100 ||
                    proportions.ReportPercentage + proportions.PresentationPercentage != 100 ||
                    proportions.FivePointPercentage < 0 || proportions.FivePointPercentage > 10 ||
                    proportions.FourPointPercentage < 0 || proportions.FourPointPercentage > 10 ||
                    proportions.ThreePointPercentage < 0 || proportions.ThreePointPercentage > 10 ||
                    proportions.FivePointPercentage + proportions.FourPointPercentage + proportions.ThreePointPercentage != 10)
                    throw new InvalidOperationException();
                var result = _classDao.Update(proportions);//新建班级时已经建了一个空的
                //if (result != 0) return -1;
                //return classId;
            }
            catch (InvalidOperationException ei) { throw ei; }
            catch (ClassNotFoundException ec){ throw ec; }
        }

        ///<summary>
        ///根据学生ID获取班级列表.
        /// @author Group ViceVersa
        ///
        /// </summary>
        /// <param name="userId">学生ID</param>
        /// <returns>list 班级列表</returns>
        /// <exception cref="T:System.ArgumentException">userId格式错误时抛出</exception>
        ///  <exception cref="T:Xmu.Crms.Shared.Exceptions.ClassesNotFoundException">无此班级</exception>
        public List<ClassInfo> ListClassByUserId(long userId)
        {
            try
            {
                return _classDao.ListClassByUserId(userId);
            }
            catch (ClassNotFoundException e) { throw e; }
        }

        ///<summary>
        ///老师发起签到.
        /// @author Group ViceVersa
        ///往location表插入一条当前讨论课班级的签到状态
        /// </summary>
        /// <param name="location">当前讨论课班级的签到状态记录 </param>
        /// <returns> 返回location表的新记录的id</returns>
        ///   <exception cref="T:Xmu.Crms.Shared.Exceptions.SeminarNotFoundException">讨论课没有找到</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.ClassesNotFoundException">无此Id的班级</exception>
        public long CallInRollById(Location location)
        {
            try
            {
                location.Seminar = _seminarService.GetSeminarBySeminarId(location.Seminar.Id);
                location.ClassInfo = GetClassByClassId(location.ClassInfo.Id);
                location.Status = 1;
                return _classDao.InsertLocation(location);

            }
            catch(SeminarNotFoundException es) { throw es; }
            catch(ClassNotFoundException ec) { throw ec; }
        }

        /// <summary>
        /// 新增老师结束签到
        /// @author Group ViceVersa
        /// 老师结束签到,修改当前讨论课班级的签到状态为已结束
        /// </summary>
        /// <param name="location">当前讨论课班级的签到状态记录</param>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.SeminarNotFoundException">讨论课没有找到</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.ClassesNotFoundException">无此Id的班级</exception>
        void EndCallRollById(long seminarId, long classId);
        public void EndCallRollById(long seminarId, long classId)
        {
            try
            {
                //_seminarService.GetSeminarBySeminarId(seminarId);
                GetClassByClassId(classId);
                
               
                _classDao.UpdateLocation(seminarId,classId);

            }
            catch (SeminarNotFoundException es) { throw es; }
            catch (ClassNotFoundException ec) { throw ec; }
        }

      
    }
}
