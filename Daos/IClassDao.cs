﻿using System.Collections.Generic;
using Xmu.Crms.Shared.Models;

namespace Xmu.Crms.Services.ViceVersa
{
    interface IClassDao
    {
        long Save(ClassInfo t);
        long InsertSelection(CourseSelection t);
        void Delete(long id);
        void DeleteSelection(long userId,long classId);
        int Update(ClassInfo t);
        List<ClassInfo> QueryAll(long id);
        ClassInfo Get(long id);
        /**
         * 根据学生ID获取班级列表.  
         * @author YeXiaona
         * @param userId 教师ID
         * @return list 班级列表
         * @see CourseService #listCourseByUserId(BigInteger userId)
         * @see ClassService #listClassByCourseId(BigInteger courseId)
         * @exception InfoIllegalException userId格式错误时抛出
         * @exception InfoIllegalException courseId格式错误时抛出
         * @exception CourseNotFoundException 未找到课程
         * @exception ClassNotFoundException 未找到班级
         */
        List<ClassInfo> ListClassByUserId(long userId);
    }
}
