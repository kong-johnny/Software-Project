using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Assist.Model
{
    public class StudentDetails
    {
        /// <summary>
        /// 通讯地址
        /// </summary>
        public string Address { get; private set; }
        /// <summary>
        /// 头像Id
        /// </summary>
        public string AvatarId { get; private set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public string Birthday { get; private set; }
        /// <summary>
        /// 班级名称
        /// </summary>
        public string ClassName { get; private set; }
        /// <summary>
        /// 院系
        /// </summary>
        public string College { get; private set; }
        /// <summary>
        /// 志愿名称
        /// </summary>
        public string CollegeWill { get; private set; }
        /// <summary>
        /// 文化程度
        /// </summary>
        public string CultureStandard { get; private set; }
        /// <summary>
        /// 培养层次
        /// </summary>
        public string EducationLevel { get; private set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; private set; }
        /// <summary>
        /// 高考号
        /// </summary>
        public string GaokaoId { get; private set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; private set; }
        /// <summary>
        /// 用户序号
        /// </summary>
        public string Id { get; private set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string IdNumber { get; private set; }
        /// <summary>
        /// 毕业中学
        /// </summary>
        public string MiddleSchool { get; private set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; private set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 民族
        /// </summary>
        public string Nationality { get; private set; }
        /// <summary>
        /// 学号
        /// </summary>
        public string Number { get; private set; }
        /// <summary>
        /// 拼音
        /// </summary>
        public string Pinyin { get; private set; }
        /// <summary>
        /// 入学年级
        /// </summary>
        public string RegistrationGrade { get; private set; }
        /// <summary>
        /// 报到时间
        /// </summary>
        public string RegistrationTime { get; private set; }
        /// <summary>
        /// 学制（x学年制）
        /// </summary>
        public string SchoolSystem { get; private set; }
        /// <summary>
        /// 专业
        /// </summary>
        public string Speciality { get; private set; }

        public StudentDetails(string address, string avatarId, string birthday,
            string className, string college, string collegeWill, string cultureStandard,
            string educationLevel, string email, string gaokaoId, string gender,
            string id, string idNumber, string middleSchool, string mobile,
            string name, string nationality, string number, string pinyin,
            string registrationGrade, string registrationTime, string schoolSystem,
            string speciality)
        {
            Address = address;
            AvatarId = avatarId;
            Birthday = birthday;
            ClassName = className;
            College = college;
            CollegeWill = collegeWill;
            CultureStandard = cultureStandard;
            EducationLevel = educationLevel;
            Email = email;
            GaokaoId = gaokaoId;
            Gender = gender;
            Id = id;
            IdNumber = idNumber;
            MiddleSchool = middleSchool;
            Mobile = mobile;
            Name = name;
            Nationality = nationality;
            Number = number;
            Pinyin = pinyin;
            RegistrationGrade = registrationGrade;
            RegistrationTime = registrationTime;
            SchoolSystem = schoolSystem;
            Speciality = speciality;
        }
    }
}
