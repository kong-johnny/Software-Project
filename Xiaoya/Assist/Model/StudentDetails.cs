using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Assist.Model
{
    public class StudentDetails
    {
        private string m_Address, m_AvatarId, m_Birthday, m_ClassName, m_College,
            m_CollegeWill, m_CultureStandard, m_EducationLevel, m_Email, m_GaokaoId,
            m_Gender, m_Id, m_IdNumber, m_MiddleSchool, m_Mobile, m_Name, m_Nationality,
            m_Number, m_Pinyin, m_RegistrationGrade, m_RegistrationTime, m_SchoolSystem,
            m_Speciality;

        /// <summary>
        /// 通讯地址
        /// </summary>
        public string Address { get => m_Address; }
        /// <summary>
        /// 头像Id
        /// </summary>
        public string AvatarId { get => m_AvatarId; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public string Birthday { get => m_Birthday; }
        /// <summary>
        /// 班级名称
        /// </summary>
        public string ClassName { get => m_ClassName; }
        /// <summary>
        /// 院系
        /// </summary>
        public string College { get => m_College; }
        /// <summary>
        /// 志愿名称
        /// </summary>
        public string CollegeWill { get => m_CollegeWill; }
        /// <summary>
        /// 文化程度
        /// </summary>
        public string CultureStandard { get => m_CultureStandard; }
        /// <summary>
        /// 培养层次
        /// </summary>
        public string EducationLevel { get => m_EducationLevel; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get => m_Email; }
        /// <summary>
        /// 高考号
        /// </summary>
        public string GaokaoId { get => m_GaokaoId; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get => m_Gender; }
        /// <summary>
        /// 用户序号
        /// </summary>
        public string Id { get => m_Id; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string IdNumber { get => m_IdNumber; }
        /// <summary>
        /// 毕业中学
        /// </summary>
        public string MiddleSchool { get => m_MiddleSchool; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get => m_Mobile; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get => m_Name; }
        /// <summary>
        /// 民族
        /// </summary>
        public string Nationality { get => m_Nationality; }
        /// <summary>
        /// 学号
        /// </summary>
        public string Number { get => m_Number; }
        /// <summary>
        /// 拼音
        /// </summary>
        public string Pinyin { get => m_Pinyin; }
        /// <summary>
        /// 入学年级
        /// </summary>
        public string RegistrationGrade { get => m_RegistrationGrade; }
        /// <summary>
        /// 报到时间
        /// </summary>
        public string RegistrationTime { get => m_RegistrationTime; }
        /// <summary>
        /// 学制（x学年制）
        /// </summary>
        public string SchoolSystem { get => m_SchoolSystem; }
        /// <summary>
        /// 专业
        /// </summary>
        public string Speciality { get => m_Speciality; }

        public StudentDetails(string address, string avatarId, string birthday,
            string className, string college, string collegeWill, string cultureStandard,
            string educationLevel, string email, string gaokaoId, string gender,
            string id, string idNumber, string middleSchool, string mobile,
            string name, string nationality, string number, string pinyin,
            string registrationGrade, string registrationTime, string schoolSystem,
            string speciality)
        {
            m_Address = address;
            m_AvatarId = avatarId;
            m_Birthday = birthday;
            m_ClassName = className;
            m_College = college;
            m_CollegeWill = collegeWill;
            m_CultureStandard = cultureStandard;
            m_EducationLevel = educationLevel;
            m_Email = email;
            m_GaokaoId = gaokaoId;
            m_Gender = gender;
            m_Id = id;
            m_IdNumber = idNumber;
            m_MiddleSchool = middleSchool;
            m_Mobile = mobile;
            m_Name = name;
            m_Nationality = nationality;
            m_Number = number;
            m_Pinyin = pinyin;
            m_RegistrationGrade = registrationGrade;
            m_RegistrationTime = registrationTime;
            m_SchoolSystem = schoolSystem;
            m_Speciality = speciality;
        }
    }
}
