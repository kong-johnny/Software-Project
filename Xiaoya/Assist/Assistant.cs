using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using CXHttpNS;
using AngleSharp.Parser.Html;
using AngleSharp.Parser.Xml;
using Xiaoya.Helpers;
using Xiaoya.Assist.Model;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Xiaoya
{
    public class Assistant
    {
        /// <summary>
        /// Headers
        /// </summary>
        private const string HEADER_USER_AGENT  = "User-Agent";
        private const string USER_AGENT         =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) " +
            "Chrome/55.0.2883.87 Safari/537.36";
        private const string HEADER_REFERER     = "Referer";
        private const string REFERER            = "http://zyfw.bnu.edu.cn";
        private const string REFERER_EXAM_SCORE = "http://zyfw.bnu.edu.cn/student/xscj.stuckcj.jsp?menucode=JW130706";
        private const string REFERER_TIMETABLE  = "http://zyfw.bnu.edu.cn/student/xkjg.wdkb.jsp?menucode=JW130418";

        /// <summary>
        /// URLs
        /// </summary>

        private const string URL_LOGIN          // 登录
            = "http://cas.bnu.edu.cn/cas/login?service=http%3A%2F%2Fzyfw.bnu.edu.cn%2FMainFrm.html";

        private const string URL_STUDENT_INFO   // 学生信息
            = "http://zyfw.bnu.edu.cn/STU_DynamicInitDataAction.do?classPath=com.kingosoft.service" +
            ".jw.student.pyfa.CourseInfoService&xn=2015&xq_m=1";

        private const string URL_GRADE_INFO     // 年级专业信息
            = "http://zyfw.bnu.edu.cn/jw/common/getStuGradeSpeciatyInfo.action";

        private const string URL_STUDENT_DETAILS    // 学生详细资料
            = "http://zyfw.bnu.edu.cn/STU_BaseInfoAction.do?" +
            "hidOption=InitData&menucode_current=JW13020101";

        private const string URL_EXAM_SCORE     // 成绩
            = "http://zyfw.bnu.edu.cn/student/xscj.stuckcj_data.jsp";

        private const string URL_DROPLIST       // 下拉列表，参见Droplists常量
            = "http://zyfw.bnu.edu.cn/frame/droplist/getDropLists.action";

        private const string URL_DATA_TABLE     // 数据表，参见Tables常量
            = "http://zyfw.bnu.edu.cn/taglib/DataTable.jsp?tableId=";

        private const string URL_TIMETABLE      // 课程表
            = "http://zyfw.bnu.edu.cn/wsxk/xkjg.ckdgxsxdkchj_data10319.jsp?params=";

        private const string URL_SELECT_INFO    // 选课信息
            = "http://zyfw.bnu.edu.cn/jw/common/getWsxkTimeRange.action?xktype=2";

        /// <summary>
        /// Droplists
        /// </summary>
        private const string DROP_EXAM_NAME = "Ms_KSSW_FBXNXQKSLC"; // 考试轮次
        private const string DROP_SEMESTER = "Ms_KBBP_FBXQLLJXAP";  // 学期

        /// <summary>
        /// Tables
        /// </summary>
        private const string TABLE_EXAM_ARRANGEMENT = "2538";       // 考试安排

        /// <summary>
        /// Other fields
        /// </summary>
        private HtmlParser m_Parser = new HtmlParser();
        private CXSession m_Session = CXHttp.Session();

        private string m_Username = "", m_Password = "";
        public string Username { get => m_Username; set { Reset();  m_Username = value; } }
        public string Password { get => m_Password; set { Reset();  m_Password = value; } }

        private bool m_IsLogined = false;
        public bool IsLogin { get => m_IsLogined; }

        private StudentInfo m_StudentInfo;
        private SelectInfo m_SelectInfo;
        private StudentDetails m_StudentDetails;

        /// <summary>
        /// Reset members
        /// </summary>
        private void Reset()
        {
            m_IsLogined      = false;
            m_StudentInfo    = null;
            m_SelectInfo     = null;
            m_StudentDetails = null;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Assistant()
        {
            Logout();
        }

        /// <summary>
        /// Constructor with username and password
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        public Assistant(string username, string password)
        {
            Logout();
            m_Username = username;
            m_Password = password;
        }

        public void Logout()
        {
            m_Username = "";
            m_Password = "";
            Reset();
            m_Session.Req.ClearCookies("http://zyfw.bnu.edu.cn");
        }

        /// <summary>
        /// Get login parameters from the login page
        /// </summary>
        /// <returns>A 2-element array of <see cref="string"/>: [0] => lt, [1] => execution</returns>
        private async Task<string[]> FetchLoginParams()
        {
            string[] retParams = new string[2];                 // there'll be 2 params

            var res = await m_Session.Req                       // GET URL_LOGIN with specific UA
               .Url(URL_LOGIN)
               .ClearCookies()
               .Header(HEADER_USER_AGENT, USER_AGENT)
               .Get();

            string body = await res.Content();                  // fetch the response body (html)

            // extract the value of param "lt"
            Match mc = Regex.Match(body, "input type=\"hidden\" name=\"lt\" value=\"(.*)\"");
            retParams[0] = mc.Groups[1].Value;

            // extract the value of param "execution"
            mc = Regex.Match(body, "input type=\"hidden\" name=\"execution\" value=\"(.*)\"");
            retParams[1] = mc.Groups[1].Value;

            return retParams;
        }

        /// <summary>
        /// Update and return IsLogin state according to the HTML body
        /// </summary>
        /// <param name="body">HTML body</param>
        /// <returns>A boolean value indicating login state</returns>
        private bool UpdateLoginState(string body)
        {
            if (body == null || body.Contains("统一身份认证平台"))
            {
                m_IsLogined = false;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get student info
        /// <list type="bullet">
        ///     <item>student_id: xh</item>
        ///     <item>grade: nj</item>
        ///     <item>major: zymc</item>
        ///     <item>major_id: zydm</item>
        ///     <item>school_year: xn</item>
        ///     <item>semester: xq_m</item>
        /// </list>
        /// </summary>
        /// <returns><see cref="StudentInfo"/><returns>
        public async Task<StudentInfo> GetStudentInfo()
        {
            if (m_StudentInfo != null)
            {
                return m_StudentInfo;
            }

            var res = await m_Session.Req
                .Url(URL_STUDENT_INFO)
                .Header(HEADER_USER_AGENT, USER_AGENT)
                .Header(HEADER_REFERER, REFERER)
                .Post();

            string body = await res.Content();

            // Update login state to avoid session expiration
            if (!UpdateLoginState(body))
                return null;

            // Parse the fetched XML body
            var xmlParser = new XmlParser();
            var doc       = xmlParser.Parse(body);

            string studentId, grade, major, majorId, schoolYear, semester;

            studentId  = ParserHelper.GetFirstElementText(doc.GetElementsByTagName("xh"));
            grade      = ParserHelper.GetFirstElementText(doc.GetElementsByTagName("nj"));
            major      = ParserHelper.GetFirstElementText(doc.GetElementsByTagName("zymc"));
            majorId    = ParserHelper.GetFirstElementText(doc.GetElementsByTagName("zydm"));
            schoolYear = ParserHelper.GetFirstElementText(doc.GetElementsByTagName("xn"));
            semester   = ParserHelper.GetFirstElementText(doc.GetElementsByTagName("xq_m"));

            // If no major or grade info
            if (majorId == null || grade == null || majorId == "" || grade == "")
            {
                // Fetch them by another way
                GradeInfo gradeInfo = await GetGradeInfo(studentId);
                major = gradeInfo.Major;
                majorId = gradeInfo.MajorId;
                grade = gradeInfo.Grade;
            }

            m_StudentInfo = new StudentInfo(studentId, grade, major, majorId, schoolYear, semester);
            return m_StudentInfo;
        }

        /// <summary>
        /// Get grade and major info
        /// </summary>
        /// <param name="studentId">Student Id</param>
        /// <returns><see cref="GradeInfo"/></returns>
        public async Task<GradeInfo> GetGradeInfo(string studentId)
        {
            var res = await m_Session.Req
                .Url(URL_GRADE_INFO)
                .Header(HEADER_USER_AGENT, USER_AGENT)
                .Header(HEADER_REFERER, REFERER)
                .Data("xh", studentId)
                .Post();

            string body = await res.Content("UTF-8");

            if (!UpdateLoginState(body))
                return null;

            RequestResult result = JsonConvert.DeserializeObject<RequestResult>(body);
            return new GradeInfo(JsonConvert.DeserializeObject<GradeInfo._GradeInfo>(result.Result));
        }

        /// <summary>
        /// Get student details
        /// </summary>
        /// <returns><see cref="StudentDetails"/></returns>
        public async Task<StudentDetails> GetStudentDetails()
        {
            if (m_StudentDetails != null)
            {
                return m_StudentDetails;
            }
            var res = await m_Session.Req
                .Url(URL_STUDENT_DETAILS)
                .Header(HEADER_USER_AGENT, USER_AGENT)
                .Header(HEADER_REFERER, REFERER)
                .Post();

            string body = await res.Content();

            if (!UpdateLoginState(body))
                return null;

            var xmlParser = new XmlParser();
            var doc       = xmlParser.Parse(body);

            var info = ParserHelper.GetFirstElement(doc.GetElementsByTagName("info"));
            if (info == null)
            {
                return null;
            }
            m_StudentDetails = new StudentDetails(
                address:            ParserHelper.GetFirstElementText(info.GetElementsByTagName("txdz")),
                avatarId:           ParserHelper.GetFirstElementText(info.GetElementsByTagName("zpid")),
                birthday:           ParserHelper.GetFirstElementText(info.GetElementsByTagName("csrq")),
                className:          ParserHelper.GetFirstElementText(info.GetElementsByTagName("bjmc")),
                college:            ParserHelper.GetFirstElementText(info.GetElementsByTagName("yxb")),
                collegeWill:        ParserHelper.GetFirstElementText(info.GetElementsByTagName("zymc")),
                cultureStandard:    ParserHelper.GetFirstElementText(info.GetElementsByTagName("whcd")),
                educationLevel:     ParserHelper.GetFirstElementText(info.GetElementsByTagName("pycc")),
                email:              ParserHelper.GetFirstElementText(info.GetElementsByTagName("dzyx")),
                gaokaoId:           ParserHelper.GetFirstElementText(info.GetElementsByTagName("gkksh")),
                gender:             ParserHelper.GetFirstElementText(info.GetElementsByTagName("xb")),
                id:                 ParserHelper.GetFirstElementText(info.GetElementsByTagName("yhxh")),
                idNumber:           ParserHelper.GetFirstElementText(info.GetElementsByTagName("sfzjh")),
                middleSchool:       ParserHelper.GetFirstElementText(info.GetElementsByTagName("sydw")),
                mobile:             ParserHelper.GetFirstElementText(info.GetElementsByTagName("dh")),
                name:               ParserHelper.GetFirstElementText(info.GetElementsByTagName("xm")),
                nationality:        ParserHelper.GetFirstElementText(info.GetElementsByTagName("mz")),
                number:             ParserHelper.GetFirstElementText(info.GetElementsByTagName("xh")),
                pinyin:             ParserHelper.GetFirstElementText(info.GetElementsByTagName("xmpy")),
                registrationGrade:  ParserHelper.GetFirstElementText(info.GetElementsByTagName("rxnj")),
                registrationTime:   ParserHelper.GetFirstElementText(info.GetElementsByTagName("bdtime")),
                schoolSystem:       ParserHelper.GetFirstElementText(info.GetElementsByTagName("xz")),
                speciality:         ParserHelper.GetFirstElementText(info.GetElementsByTagName("lqzy"))
            );
            return m_StudentDetails;
        }

        /// <summary>
        /// Login to the BNU universal authentication platform
        /// </summary>
        /// <returns>Error message. Returns <c>null</c> if succeed.</returns>
        public async Task<string> Login()
        {
            if (m_IsLogined) return null;
            // Fetch login params needed
            string[] loginParams = await FetchLoginParams();

            var res = await m_Session.Req
                .Url(URL_LOGIN)
                .Header(HEADER_USER_AGENT, USER_AGENT)
                .Data("username", m_Username)
                .Data("password", m_Password)
                .Data("code", "code")
                .Data("lt", loginParams[0])
                .Data("execution", loginParams[1])
                .Data("_eventId", "submit")
                .Post();

            // Decode html body by GBK
            string body = "";
            body = res.Content("GBK").Result;

            var doc = m_Parser.Parse(body);

            // Init error message
            string error = "登录失败";

            // If no "KINGOSOFT高校数字校园综合管理平台" found, then there will be errors
            if (!body.Contains("KINGOSOFT高校数字校园综合管理平台"))
            {
                body = res.Content("UTF-8").Result;
                doc = m_Parser.Parse(body);
                // Get error message element: <span id="error_message_show">
                var msg = doc.GetElementById("msg");
                // Element found, then assign error message
                if (msg != null)
                {
                    error = msg.TextContent;
                }
                return error;
            }
            // Otherwise, logined successfully.
            m_IsLogined = true;
            return null;
        }

        /// <summary>
        /// Get exam rounds
        /// </summary>
        /// <returns>A list of <see cref="ExamRound"/></returns>
        public async Task<List<ExamRound>> GetExamRounds()
        {
            var res = await m_Session.Req
                .Url(URL_DROPLIST)
                .Header(HEADER_USER_AGENT, USER_AGENT)
                .Header(HEADER_REFERER, REFERER)
                .Data("comboBoxName", DROP_EXAM_NAME)
                .Post();

            string body = await res.Content("UTF-8");

            if (!UpdateLoginState(body))
                return null;

            var list = JsonConvert.DeserializeObject<List<ExamRound>>(body);
            return list.OrderByDescending(o => o.Name).ToList();
        }

        /// <summary>
        /// Get exam scores
        /// </summary>
        /// <param name="year">Specific which year to query. If 0 is given, all scores will be returned.</param>
        /// <param name="semester">Specific which term to query.</param>
        /// <param name="showMajor">Specific whether scores of minor profession will be returned.</param>
        /// <returns>A list of <see cref="ExamScore"/></returns>
        public async Task<List<ExamScore>> GetExamScores(int year, int semester, bool showMajor)
        {
            var req = m_Session.Req
                .Url(URL_EXAM_SCORE)
                .Header(HEADER_USER_AGENT, USER_AGENT)
                .Header(HEADER_REFERER, REFERER_EXAM_SCORE)
                .Data("ysyx", "yscj")
                .Data("userCode", (await GetStudentInfo()).StudentId)
                .Data("zfx", showMajor ? "0" : "1")
                .Data("ysyxS", "on")
                .Data("sjxzS", "on")
                .Data("zfxS", "on");

            if (year == 0)
            {
                // If year == 0, Then get all exam scores
                req.Data("sjxz", "sjxz1");
            }
            else
            {
                // Else get scores for specific semester
                req.Data("sjxz", "sjxz3")
                    .Data("xn", year.ToString())
                    .Data("xn1", (year + 1).ToString())
                    .Data("xq", semester.ToString());
            }
            var res = await req.Post();

            string body = await res.Content("GBK");

            if (!UpdateLoginState(body))
                return null;

            var doc = m_Parser.Parse(body);

            var scores = new List<ExamScore>();

            if (doc.GetElementsByTagName("tbody").Count() == 0)
            {
                // No result
                return scores;
            }

            var table = doc.GetElementsByTagName("tbody")[0];
            var rows  = table.GetElementsByTagName("tr");
            string lastTerm = "";

            foreach (var tr in rows)
            {
                var cols = tr.GetElementsByTagName("td");

                string currentTerm = cols[0].TextContent.Trim();
                if (currentTerm == "")
                {
                    currentTerm = lastTerm;
                }
                else
                {
                    lastTerm = currentTerm;
                }

                if (cols.Count() < 9) continue;

                scores.Add(new ExamScore(
                        semester:               currentTerm,
                        courseName:             cols[1].TextContent,
                        courseCredit:           cols[2].TextContent,
                        classification:         cols[3].TextContent,
                        score1:                 cols[5].TextContent,
                        score2:                 cols[6].TextContent,
                        score:                  cols[7].TextContent,
                        doLearnForFirstTime:    "初修" == cols[4].TextContent.Trim(),
                        isMajor:                "主修" == cols[8].TextContent.Trim()
                ));
            }

            return scores.OrderBy(o => o.Score).ToList();
        }

        /// <summary>
        /// Get exam scores of all semesters
        /// </summary>
        /// <param name="isOnlyMajor">Specific whether scores of minor profession will be returned.</param>
        /// <returns>A list of <see cref="ExamScore"/></returns>
        public async Task<List<ExamScore>> GetExamScores(bool isOnlyMajor)
        {
            return await GetExamScores(0, 0, isOnlyMajor);
        }

        /// <summary>
        /// Get exam arrangement of specific exam round
        /// </summary>
        /// <param name="round"><see cref="ExamRound"/></param>
        /// <returns>A list of <see cref="ExamArrangement"/></returns>
        public async Task<List<ExamArrangement>> GetExamArrangement(ExamRound round)
        {
            var res = await m_Session.Req
                .Url(URL_DATA_TABLE + TABLE_EXAM_ARRANGEMENT)
                .Header(HEADER_USER_AGENT, USER_AGENT)
                .Header(HEADER_REFERER, REFERER)
                .Data("xh", "")
                .Data("xn", round.Year)
                .Data("xq", round.Semester)
                .Data("kslc", round.Round)
                .Data("xnxqkslc", round.Code)
                .Post();

            string body = await res.Content("GBK");

            if (!UpdateLoginState(body))
                return null;

            var doc = m_Parser.Parse(body);

            var arrangementList = new List<ExamArrangement>();

            for (int i = 0; ; ++i)
            {
                string prefix = "tr" + i + "_";

                var courseNameEl = doc.GetElementById(prefix + "kc");
                if (courseNameEl == null) {
                    // IF no tr{i}_kc, THEN no new lines.
                    break;
                }

                ExamArrangement arrangement = new ExamArrangement(
                    courseName:     courseNameEl.TextContent,
                    credit:         doc.GetElementById(prefix + "xf").TextContent,
                    classification: doc.GetElementById(prefix + "lb").TextContent,
                    examType:       doc.GetElementById(prefix + "khfs").TextContent,
                    time:           doc.GetElementById(prefix + "kssj").TextContent,
                    location:       doc.GetElementById(prefix + "ksdd").TextContent,
                    seat:           doc.GetElementById(prefix + "zwh").TextContent
                );
                arrangementList.Add(arrangement);
            }

            arrangementList.Sort((a, b) =>
            {
                if (a.BeginTime.HasValue && a.EndTime.HasValue &&
                    b.BeginTime.HasValue && b.EndTime.HasValue)
                {
                    bool aHasEnded = a.EndTime.Value < DateTime.Now;
                    bool bHasEnded = b.EndTime.Value < DateTime.Now;
                    if (aHasEnded && bHasEnded)
                    {
                        return b.EndTime.Value.CompareTo(a.EndTime.Value);
                    }
                    else if (!aHasEnded && !bHasEnded)
                    {
                        return a.BeginTime.Value.CompareTo(b.BeginTime.Value);
                    }
                    else if (aHasEnded && !bHasEnded)
                    {
                        return 1000;
                    }
                    else
                    {
                        return -1000;
                    }
                }
                return 1;
            });

            return arrangementList;
        }

        /// <summary>
        /// Get table semesters
        /// </summary>
        /// <returns>A list of <see cref="TableSemester"/></returns>
        public async Task<List<TableSemester>> GetTableSemesters()
        {
            var res = await m_Session.Req
                .Url(URL_DROPLIST)
                .Header(HEADER_USER_AGENT, USER_AGENT)
                .Header(HEADER_REFERER, REFERER)
                .Data("comboBoxName", DROP_SEMESTER)
                .Post();

            string body = await res.Content("UTF-8");

            if (!UpdateLoginState(body))
                return null;

            return JsonConvert.DeserializeObject<List<TableSemester>>(body);
        }

        /// <summary>
        /// Get courses of timetable for specific semester
        /// </summary>
        /// <param name="semester"><see cref="TableSemester"/></param>
        /// <returns>A list of <see cref="TableCourse"/></returns>
        public async Task<TableCourses> GetTableCourses(TableSemester semester)
        {
            // Base64 encoding content
            string content = "xn=" + semester.Year
                + "&xq=" + semester.Semester
                + "&xh=" + (await GetStudentInfo()).StudentId;

            content = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(content));

            var res = await m_Session.Req
                .Url(URL_TIMETABLE + content)
                .Header(HEADER_USER_AGENT, USER_AGENT)
                .Header(HEADER_REFERER, REFERER_TIMETABLE)
                .Get();

            string body = await res.Content();

            if (!UpdateLoginState(body))
                return null;

            var doc = m_Parser.Parse(body);

            var courses = new List<TableCourse>();

            var table = ParserHelper.GetFirstElement(doc.GetElementsByTagName("tbody"));
            if (table != null)
            {
                var rows = table.GetElementsByTagName("tr");
                foreach (var tr in rows)
                {
                    var cols = tr.GetElementsByTagName("td");
                    if (cols.Count() < 14) continue;
                    courses.Add(new TableCourse(
                        code: cols[13].TextContent,
                        name: cols[0].TextContent,
                        credit: cols[2].TextContent,
                        teacher: cols[4].TextContent,
                        locationTime: cols[5].TextContent,
                        isFreeToListen: cols[8].TextContent == "是"
                    ));
                }
            }

            return new TableCourses((await GetStudentDetails()).Name  + " (" + semester.Code + ")", courses);
        }
        
        public async Task<SelectInfo> GetSelectInfo()
        {
            if (m_SelectInfo != null)
            {
                return m_SelectInfo;
            }

            var res = await m_Session.Req
                .Url(URL_SELECT_INFO)
                .Header(HEADER_USER_AGENT, USER_AGENT)
                .Header(HEADER_REFERER, REFERER_TIMETABLE)
                .Post();

            string body = await res.Content("UTF-8");

            if (!UpdateLoginState(body))
                return null;

            RequestResult result = JsonConvert.DeserializeObject<RequestResult>(body);
            return JsonConvert.DeserializeObject<SelectInfo>(result.Result);
        }
    }
}
