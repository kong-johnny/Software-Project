using LeanCloud;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Xiaoya.Assist.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Xiaoya.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExamScorePage : Page
    {
        private App app = (App)Application.Current;

        private List<ExamScore> Scores;
        private List<ExamRound> Round = new List<ExamRound>();

        private ExamRound currentRound = null;

        bool showMajor = true;

        public ExamScorePage()
        {
            this.InitializeComponent();
            this.Loaded += ExamScorePage_Loaded;
        }

        private async void ExamScorePage_Loaded(object sender, RoutedEventArgs e)
        {
            double? diagonal = DisplayInformation.GetForCurrentView().DiagonalSizeInInches;

            //move commandbar to page bottom on small screens
            if (diagonal < 7)
            {
                topbar.Visibility = Visibility.Collapsed;
                pageTitleContainer.Visibility = Visibility.Visible;
                bottombar.Visibility = Visibility.Visible;
            }
            else
            {
                topbar.Visibility = Visibility.Visible;
                pageTitleContainer.Visibility = Visibility.Collapsed;
                bottombar.Visibility = Visibility.Collapsed;
            }

            if (!app.Assist.IsLogin)
            {
                var msgDialog = new CommonDialog
                {
                    Title = "提示",
                    Message = "请先登录！",
                    CloseButtonText = "确定"
                };

                await msgDialog.ShowAsyncQueue();
                if (Frame.CanGoBack)
                {
                    Frame.GoBack();
                }
            }
            else
            {
                LoadingProgressBar.Visibility = Visibility.Visible;

                try
                {

                    var studentInfo = await app.Assist.GetStudentInfo();

                    for (int year = DateTime.Now.Year;
                        year >= Convert.ToInt32(studentInfo.Grade);
                        --year)
                    {
                        if (year == DateTime.Now.Year)
                        {
                            if (DateTime.Now.Month > 9)
                            {
                                Round.Add(new ExamRound("" + year + "-" + (year + 1) + " 秋季学期", year + ",0,0"));
                            }
                        }
                        else
                        {
                            Round.Add(new ExamRound("" + year + "-" + (year + 1) + " 春季学期", year + ",1,0"));
                            Round.Add(new ExamRound("" + year + "-" + (year + 1) + " 秋季学期", year + ",0,0"));
                        }
                    }

                    Round.Add(new ExamRound("全部学期", "0,0,0"));

                    SemesterComboBox.ItemsSource = Round;
                    SemesterComboBox.SelectionChanged += SemesterComboBox_SelectionChanged;

                    LoadingProgressBar.Visibility = Visibility.Collapsed;

                    SemesterComboBox.SelectedItem = Round[0];
                }
                catch (Exception err)
                {
                    LoadingProgressBar.Visibility = Visibility.Collapsed;
                    var msgDialog = new CommonDialog
                    {
                        Title = "错误",
                        Message = err.Message,
                        CloseButtonText = "确定"
                    };

                    await msgDialog.ShowAsyncQueue();
                }

                // Prepare for Sharing
                DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
                dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            }
        }

        private async void SaveScores()
        {
            try
            {
                var studentId = (await app.Assist.GetStudentInfo()).StudentId;

                var query = new AVQuery<AVObject>("CourseScore").WhereEqualTo("stuKey", studentId);

                var objs = await query.FindAsync();
                List<AVObject> saving = new List<AVObject>();

                foreach (var score in Scores)
                {
                    bool has = false;
                    var fullName = "[" + score.CourseId + "]" + score.CourseName;
                    foreach (var obj in objs)
                    {
                        if (Convert.ToString(obj["courseName"]) == fullName)
                        {
                            has = true;
                            break;
                        }
                    }

                    if (has)
                    {
                        continue;
                    }


                    if (Double.TryParse(score.Score1, out double dScore1) &&
                        Double.TryParse(score.Score2, out double dScore2))
                    {
                        var o = new AVObject("CourseScore")
                        {
                            ["stuKey"] = studentId,
                            ["courseName"] = fullName,
                            ["term"] = score.Semester,
                            ["score1"] = dScore1,
                            ["score2"] = dScore2,
                            ["score"] = score.Score
                        };
                        saving.Add(o);
                    }
                }
                await AVObject.SaveAllAsync(saving);
            }
            catch
            {

            }
        }

        private async void SemesterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SemesterComboBox.SelectedItem != null)
            {
                LoadingProgressBar.Visibility = Visibility.Visible;

                try
                {

                    currentRound = (ExamRound)SemesterComboBox.SelectedItem;

                    Scores = await app.Assist.GetExamScores(
                        Convert.ToInt32(currentRound.Year),
                        Convert.ToInt32(currentRound.Semester),
                        showMajor
                    );
                    ExamScoreListView.ItemsSource = Scores;
                }
                catch (Exception err)
                {
                    var msgDialog = new CommonDialog
                    {
                        Title = "错误",
                        Message = err.Message,
                        CloseButtonText = "确定"
                    };

                    await msgDialog.ShowAsyncQueue();
                }
                finally
                {
                    LoadingProgressBar.Visibility = Visibility.Collapsed;
                }

                SaveScores();
            }
        }


        private async void ExamScoreListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = (ExamScore)e.ClickedItem;
            var msgDialog = new CommonDialog
            {
                Title = item.CourseName,
                Message = "课程编号：" + item.CourseId +
                            "\n学期：" + item.Semester +
                            "\n学分：" + item.CourseCredit +
                            "\n类别：" + item.Classification +
                            "\n是否主修：" + (item.IsMajor ? "是" : "否") +
                            "\n是否初修：" + (item.DoLearnForFirstTime ? "是" : "否") +
                            "\n\n平时成绩：" + item.Score1 +
                            "\n期末成绩：" + item.Score2 +
                            "\n综合成绩：" + item.Score,
                CloseButtonText = "确定"
            };

            await msgDialog.ShowAsyncQueue();
        }

        private async void Refresh()
        {
            if (LoadingProgressBar != null && currentRound != null && app.Assist != null && ExamScoreListView != null)
            {
                LoadingProgressBar.Visibility = Visibility.Visible;

                try
                {

                    Scores = await app.Assist.GetExamScores(
                        Convert.ToInt32(currentRound.Year),
                        Convert.ToInt32(currentRound.Semester),
                        showMajor
                    );
                    ExamScoreListView.ItemsSource = Scores;
                }
                catch (Exception err)
                {
                    var msgDialog = new CommonDialog
                    {
                        Title = "错误",
                        Message = err.Message,
                        CloseButtonText = "确定"
                    };

                    await msgDialog.ShowAsyncQueue();
                }
                finally
                {
                    LoadingProgressBar.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ShowMajor_Checked(object sender, RoutedEventArgs e)
        {
            showMajor = true;
            Refresh();
        }

        private void ShowMajor_Unchecked(object sender, RoutedEventArgs e)
        {
            showMajor = false;
            Refresh();
        }

        private class GPAResult
        {
            public double AverageGPA { get; private set; }
            public double StandardGPA { get; private set; }
            public double ImprovedGPA1 { get; private set; }
            public double ImprovedGPA2 { get; private set; }
            public double PKUGPA { get; private set; }

            public string AverageGPAText { get => string.Format("{0:0.00}", AverageGPA); }
            public string StandardGPAText { get => string.Format("{0:0.00}", StandardGPA); }
            public string ImprovedGPAText1 { get => string.Format("{0:0.00}", ImprovedGPA1); }
            public string ImprovedGPAText2 { get => string.Format("{0:0.00}", ImprovedGPA2); }
            public string PKUGPAText { get => string.Format("{0:0.00}", PKUGPA); }

            public GPAResult(double averageGPA, double standardGPA,
                double improvedGPA1, double improvedGPA2, double PKUGPA)
            {
                AverageGPA = averageGPA;
                StandardGPA = standardGPA;
                ImprovedGPA1 = improvedGPA1;
                ImprovedGPA2 = improvedGPA2;
                this.PKUGPA = PKUGPA;
            }
        }

        private GPAResult CalculateGPA(List<ExamScore> scores)
        {
            double totalScores = 0,
                totalStandardScores = 0,
                totalImprovedScores1 = 0,
                totalImprovedScores2 = 0,
                totalPKUScores = 0;

            double totalCredits = 0;

            foreach (var score in scores)
            {
                totalScores += score.NumericScore * score.CourseCredit;
                totalStandardScores += score.StandardFourPointsGPA * score.CourseCredit;
                totalImprovedScores1 += score.ImprovedFourPointsGPA1 * score.CourseCredit;
                totalImprovedScores2 += score.ImprovedFourPointsGPA2 * score.CourseCredit;
                totalPKUScores += score.PKUFourPointsGPA * score.CourseCredit;

                totalCredits += score.CourseCredit;
            }

            return new GPAResult(
                totalScores / totalCredits,
                totalStandardScores / totalCredits,
                totalImprovedScores1 / totalCredits,
                totalImprovedScores2 / totalCredits,
                totalPKUScores / totalCredits
            );
        }

        private async void GPAReport_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadingProgressBar.Visibility = Visibility.Visible;

                // Scores
                var scores = await app.Assist.GetExamScores(true);
                // Content of GPA Report
                string report = "";

                // Overall GPA
                report += " # 总GPA \n\n";
                {
                    var values = scores;
                    var GPA = CalculateGPA(values);

                    report += "标准加权算法：\t\t" + GPA.AverageGPAText + "  \n";
                    report += "标准四分制算法：\t" + GPA.StandardGPAText + "  \n";
                    report += "改良(1)四分制算法：\t" + GPA.ImprovedGPAText1 + "  \n";
                    report += "改良(2)四分制算法：\t" + GPA.ImprovedGPAText2 + "  \n";
                    report += "北大四分制算法：\t" + GPA.PKUGPAText + "  \n";

                    report += "\n ## 除公共课外\n\n";

                    values = values.FindAll(x => x.IsMajorCourse);
                    GPA = CalculateGPA(values);

                    report += "标准加权算法：\t\t" + GPA.AverageGPAText + "  \n";
                    report += "标准四分制算法：\t" + GPA.StandardGPAText + "  \n";
                    report += "改良(1)四分制算法：\t" + GPA.ImprovedGPAText1 + "  \n";
                    report += "改良(2)四分制算法：\t" + GPA.ImprovedGPAText2 + "  \n";
                    report += "北大四分制算法：\t" + GPA.PKUGPAText + "  \n";

                    report += "\n --- \n\n";
                }

                // GPA for each years
                var scoreByYear = scores.GroupBy(o => o.Semester.Substring(0, 11));
                scoreByYear = scoreByYear.OrderByDescending(x => x.Key);

                report += " # 最后两学年GPA \n\n";
                {
                    List<ExamScore> lastTwoYearsScores = new List<ExamScore>();
                    int yearIndex = 0;
                    foreach (var year in scoreByYear)
                    {
                        if (yearIndex++ == 2) break;
                        lastTwoYearsScores.AddRange(year);
                    }
                    var GPA = CalculateGPA(lastTwoYearsScores);

                    report += "标准加权算法：\t\t" + GPA.AverageGPAText + "  \n";
                    report += "标准四分制算法：\t" + GPA.StandardGPAText + "  \n";
                    report += "改良(1)四分制算法：\t" + GPA.ImprovedGPAText1 + "  \n";
                    report += "改良(2)四分制算法：\t" + GPA.ImprovedGPAText2 + "  \n";
                    report += "北大四分制算法：\t" + GPA.PKUGPAText + "  \n";

                    report += "\n ## 除公共课外\n\n";

                    lastTwoYearsScores = lastTwoYearsScores.FindAll(x => x.IsMajorCourse);
                    GPA = CalculateGPA(lastTwoYearsScores);

                    report += "标准加权算法：\t\t" + GPA.AverageGPAText + "  \n";
                    report += "标准四分制算法：\t" + GPA.StandardGPAText + "  \n";
                    report += "改良(1)四分制算法：\t" + GPA.ImprovedGPAText1 + "  \n";
                    report += "改良(2)四分制算法：\t" + GPA.ImprovedGPAText2 + "  \n";
                    report += "北大四分制算法：\t" + GPA.PKUGPAText + "  \n";

                    report += "\n --- \n\n";
                }

                report += " # 分学年GPA \n\n";
                foreach (var year in scoreByYear)
                {
                    report += " ## " + year.Key + "\n\n";

                    var values = year.Select(x => x).ToList();

                    var GPA = CalculateGPA(values);

                    report += "标准加权算法：\t\t" + GPA.AverageGPAText + "  \n";
                    report += "标准四分制算法：\t" + GPA.StandardGPAText + "  \n";
                    report += "改良(1)四分制算法：\t" + GPA.ImprovedGPAText1 + "  \n";
                    report += "改良(2)四分制算法：\t" + GPA.ImprovedGPAText2 + "  \n";
                    report += "北大四分制算法：\t" + GPA.PKUGPAText + "  \n";

                    report += "\n ### 除公共课外\n\n";

                    values = values.FindAll(x => x.IsMajorCourse);
                    GPA = CalculateGPA(values);

                    report += "标准加权算法：\t\t" + GPA.AverageGPAText + "  \n";
                    report += "标准四分制算法：\t" + GPA.StandardGPAText + "  \n";
                    report += "改良(1)四分制算法：\t" + GPA.ImprovedGPAText1 + "  \n";
                    report += "改良(2)四分制算法：\t" + GPA.ImprovedGPAText2 + "  \n";
                    report += "北大四分制算法：\t" + GPA.PKUGPAText + "  \n";

                    report += "\n --- \n\n";
                }

                // GPA for each semesters
                report += " # 分学期GPA \n\n";
                var scoreBySemester = scores.GroupBy(o => o.Semester);
                scoreBySemester = scoreBySemester.OrderByDescending(x => x.Key);

                foreach (var semester in scoreBySemester)
                {
                    report += " ## " + semester.Key + "\n\n";

                    var values = semester.Select(x => x).ToList();

                    var GPA = CalculateGPA(values);

                    report += "标准加权算法：\t\t" + GPA.AverageGPAText + "  \n";
                    report += "标准四分制算法：\t" + GPA.StandardGPAText + "  \n";
                    report += "改良(1)四分制算法：\t" + GPA.ImprovedGPAText1 + "  \n";
                    report += "改良(2)四分制算法：\t" + GPA.ImprovedGPAText2 + "  \n";
                    report += "北大四分制算法：\t" + GPA.PKUGPAText + "  \n";

                    report += "\n ### 除公共课外\n\n";

                    values = values.FindAll(x => x.IsMajorCourse);
                    GPA = CalculateGPA(values);

                    report += "标准加权算法：\t\t" + GPA.AverageGPAText + "  \n";
                    report += "标准四分制算法：\t" + GPA.StandardGPAText + "  \n";
                    report += "改良(1)四分制算法：\t" + GPA.ImprovedGPAText1 + "  \n";
                    report += "改良(2)四分制算法：\t" + GPA.ImprovedGPAText2 + "  \n";
                    report += "北大四分制算法：\t" + GPA.PKUGPAText + "  \n";

                    report += "\n --- \n\n";
                }
                LoadingProgressBar.Visibility = Visibility.Collapsed;

                GPADialog dialog = new GPADialog(report);
                await dialog.ShowAsyncQueue();
            }
            catch (Exception err)
            {
                LoadingProgressBar.Visibility = Visibility.Collapsed;
                var msgDialog = new CommonDialog
                {
                    Title = "错误",
                    Message = err.Message,
                    CloseButtonText = "确定"
                };

                await msgDialog.ShowAsyncQueue();
            }
        }

        private void Share_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTransferManager.ShowShareUI();
            }
            catch
            { }
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            if (currentRound != null)
            {
                DataRequest request = args.Request;
                string content = currentRound.Name + "\n---\n\n";
                foreach (var score in Scores)
                {
                    content += score.CourseName +
                        "\n最终成绩：" + score.Score +
                        "\n平时：" + score.Score1 + "\t期末：" + score.Score2 + "\n\n";
                }
                request.Data.SetText(content);
                request.Data.Properties.Title = "成绩分享 - " + currentRound.Name;
            }
        }
    }
}
