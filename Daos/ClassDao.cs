using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;

namespace Xmu.Crms.Services.ViceVersa
{
    /// <summary>
    /// @author Group ViceVersa
    /// @version 2.00
    /// </summary>
    class ClassDao : IClassDao
    {
        private readonly CrmsContext _db;

        /// <summary>
        /// 构造函数 dbContext注入
        /// @author Group ViceVersa
        /// </summary>
        public ClassDao(CrmsContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 按班级id删除班级.
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="id">班级Id</param>
        public void Delete(long id)
        {
            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {
                    ClassInfo c = _db.ClassInfo.Where(u => u.Id == id).SingleOrDefault<ClassInfo>();
                    if (c == null) throw new ClassNotFoundException();

                    //根据class信息删除courseSelection表
                    DeleteSelection(0, c.Id);

                    _db.ClassInfo.Attach(c);
                    _db.ClassInfo.Remove(c);
                    _db.SaveChanges();
                    scope.Commit();
                }
                catch (ClassNotFoundException e) { scope.Rollback(); throw e; }

            }
        }

        /// <summary>
        /// 按班级id查找班级信息.
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="id">班级Id</param>
        /// <returns>ClassInfo 班级信息</returns>
        public ClassInfo Get(long id)
        {

            ClassInfo classinfo = _db.ClassInfo.Include(u=>u.Course).Include(u=>u.Course.Teacher).Where(u => u.Id == id).SingleOrDefault<ClassInfo>();
            if (classinfo == null)
            {
                throw new ClassNotFoundException();
            }
            return classinfo;

        }


        /// <summary>
        /// 根据课程id列出所有班级.
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="id">课程Id</param>
        /// <returns>list 班级列表</returns>
        public List<ClassInfo> QueryAll(long id)
        {
            //找到这门课
            Course course= _db.Course.SingleOrDefault(u => u.Id == id);
            if (course == null) { throw new CourseNotFoundException(); }


            List<ClassInfo> list = _db.ClassInfo.Include(u => u.Course).Include(u => u.Course.Teacher).Include(u=>u.Course.Teacher.School).Where(u => u.Course.Id == id).ToList<ClassInfo>();
            return list;
        }


        /// <summary>
        /// 添加学生选课表返回id.
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="t">选课信息</param>
        /// <returns>courseSelectionId 选课记录id</returns>
        public long InsertSelection(CourseSelection t)
        {
            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {

                    _db.CourseSelection.Add(t);

                    _db.SaveChanges();

                    scope.Commit();
                    return t.Id;
                }
                catch { scope.Rollback(); throw; }
            }

        }

        /// <summary>
        /// 查询学生选课表的记录.
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="classId">班级id</param>
        /// <returns>查询结果</returns>
        public int GetSelection(long userId, long classId)
        {
           

            CourseSelection courseSelection= _db.CourseSelection.Where(u=>u.ClassInfo.Id==classId&&u.Student.Id==userId).SingleOrDefault<CourseSelection>();
            if (courseSelection != null) return 1;//找到记录
            return 0;
        }

        /// <summary>
        /// 修改班级信息.
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="t">修改后班级信息</param>
        public int Update(ClassInfo t)
        {
            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {

                    ClassInfo c = _db.ClassInfo.Where(u => u.Id == t.Id).SingleOrDefault<ClassInfo>();
                    if (c == null) throw new ClassNotFoundException();
                    c.Name = t.Name;
                    c.Course = t.Course;
                    c.Site = t.Site;
                    c.ClassTime = t.ClassTime;
                    c.ReportPercentage = t.ReportPercentage;
                    c.PresentationPercentage = t.PresentationPercentage;
                    c.FivePointPercentage = t.FivePointPercentage;
                    c.FourPointPercentage = t.FourPointPercentage;
                    c.ThreePointPercentage = t.ThreePointPercentage;

                    _db.Entry(c).State = EntityState.Modified;
                    _db.SaveChanges();

                    scope.Commit();
                    return 0;
                }
                catch(ClassNotFoundException e) { scope.Rollback();throw e; }
            }
            
        }


        /// <summary>
        /// 删除学生选课表的记录.
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="classId">班级id</param>
        public void DeleteSelection(long userId, long classId)
        {
            if (userId != 0)//单个学生取消选课
            {
                using (var scope = _db.Database.BeginTransaction())
                {
                    try
                    {
                        CourseSelection c = _db.CourseSelection.SingleOrDefault<CourseSelection>(u => u.Student.Id == userId && u.ClassInfo.Id == classId);

                        _db.CourseSelection.Attach(c);
                        _db.CourseSelection.Remove(c);
                        _db.SaveChanges();
                        scope.Commit();
                    }
                    catch { scope.Rollback(); }
                }
            }

            else  //删除班级时 批量删除
            {
                List<CourseSelection> t1 = _db.CourseSelection.Where(t => t.ClassInfo.Id == classId).ToList<CourseSelection>();
                foreach (CourseSelection t in t1)
                {
                    _db.CourseSelection.Remove(t);
                }
                
                _db.SaveChanges();
            }
        }

        ///<summary>
        ///根据学生ID获取班级列表.
        /// @author Group ViceVersa
        ///
        /// </summary>
        /// <param name="userId">学生ID</param>
       /// <returns>list 班级列表</returns>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.ClassesNotFoundException">无此选课记录</exception>
        public List<ClassInfo> ListClassByUserId(long userId)
        {
            List<CourseSelection> selectionList = _db.CourseSelection.Include(c => c.Student).Include(c=>c.Student.School).Include(c => c.ClassInfo).Include(c=>c.ClassInfo.Course.Teacher.School).Where(c => c.Student.Id == userId).ToList<CourseSelection>();
            //找不到对应的选课信息
            if (selectionList == null)
                throw new ClassNotFoundException();

            //根据classId获得对应的class
            List<ClassInfo> classList = new List<ClassInfo>();
            foreach (CourseSelection i in selectionList)
                classList.Add(Get(i.ClassInfo.Id));
            return classList;
        }

        /// <summary>
        /// 老师获取位置信息，获取班级签到状态.
        /// 
        /// @author Group ViceVersa
        /// </summary>
        /// <param name="seminarId">讨论课id</param>
        /// <param name="classId">班级id</param>
        /// <returns>location 班级签到状态</returns>
        public Location GetLocation(long seminarId, long classId)
        {
            return _db.Location.Include(u=>u.ClassInfo).Include(u=>u.Seminar).SingleOrDefault<Location>(u => u.Seminar.Id == seminarId && u.ClassInfo.Id == classId);
        }

        ///<summary>
        ///老师发起签到.
        /// @author Group ViceVersa
        ///往location表插入一条当前讨论课班级的签到状态
        /// </summary>
        /// <param name="t">当前讨论课班级的签到状态记录 </param>
        /// <returns> 返回location表的新记录的id</returns>
        public long InsertLocation(Location t)
        {
            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {
                    _db.Location.Add(t);

                    _db.SaveChanges();

                    scope.Commit();
                    return t.Id;
                }
                catch { scope.Rollback(); throw; }
            }

        }

        /// <summary>
        /// 新增老师结束签到
        /// @author Group ViceVersa
        /// 老师结束签到,修改当前讨论课班级的签到状态为已结束
        /// </summary>
        /// <param name="seminarId">讨论课id</param>
        /// <param name="classId">班级id</param>
       /// <exception cref="T:Xmu.Crms.Shared.Exceptions.ClassesNotFoundException">无此Id的班级</exception>
        public int UpdateLocation(long seminarId, long classId)
        {
            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {
                    Location location= _db.Location.Include(u=>u.Seminar).Include(u=>u.ClassInfo).SingleOrDefault<Location>(u=>u.ClassInfo.Id==classId&&u.Seminar.Id==seminarId);
                    //没有记录
                    if (location == null) throw new ClassNotFoundException();

                    location.Status = 0;
                    _db.Entry(location).State = EntityState.Modified;
                    _db.SaveChanges();

                    scope.Commit();
                    return 0;
                }
                catch { scope.Rollback(); throw new ClassNotFoundException(); }
            }
        }
    }
}