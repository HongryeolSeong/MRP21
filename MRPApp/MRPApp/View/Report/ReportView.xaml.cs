﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MRPApp.View.Report
{
    /// <summary>
    /// MyAccount.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ReportView : Page
    {
        public ReportView()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                InitControls();
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 ReportView Loaded : {ex}");
                throw ex;
            }
        }

        #region 이벤트 작업 영역
        // 검색: DatePicker의 날짜를 검색 후 표시한다.
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (IsValidInputs())
            {
                var startDate = ((DateTime)DtpSearchStartDate.SelectedDate).ToString("yyyy-MM-dd");
                var endDate = ((DateTime)DtpSearchEndDate.SelectedDate).ToString("yyyy-MM-dd");
                var searchResult = Logic.DataAccess.GetReportDatas(startDate, endDate, Commons.PLANTCODE);

                DisplayChart(searchResult);
            }
        }
        #endregion

        #region 이벤트에 사용되는 메서드
        // 입력데이터 검증 메서드
        // - 날짜가 빠져있거나, StartDate가 EndDate보다 최신인 경우 배제
        private bool IsValidInputs()
        {
            var result = true;

            if (DtpSearchStartDate.SelectedDate == null || DtpSearchEndDate.SelectedDate == null)
            {
                Commons.ShowMessageAsync("검색", "검색 할 일자를 선택하세요.");
                result = false;
            }

            if (DtpSearchStartDate.SelectedDate > DtpSearchEndDate.SelectedDate)
            {
                Commons.ShowMessageAsync("검색", "시작 일자가 종료 일자보다 최신 일 수 없습니다.");
                result = false;
            }

            return result;
        }

        // 차트 출력
        private void DisplayChart(List<Model.Report> list)
        {
            int[] schAmounts = list.Select(a => (int)a.SchAmount).ToArray();
            int[] prcOkAmounts = list.Select(a => (int)a.PrcOkAmount).ToArray();
            int[] prcFailAmounts = list.Select(a => (int)a.PrcFailAmount).ToArray();

            var series1 = new LiveCharts.Wpf.ColumnSeries
            {
                Title = "계획수량",
                Fill = new SolidColorBrush(Colors.Green),
                Values = new LiveCharts.ChartValues<int>(schAmounts)
            };
            var series2 = new LiveCharts.Wpf.ColumnSeries
            {
                Title = "성공수량",
                Fill = new SolidColorBrush(Colors.Blue),
                Values = new LiveCharts.ChartValues<int>(prcOkAmounts)
            };
            var series3 = new LiveCharts.Wpf.ColumnSeries
            {
                Title = "실패수량",
                Fill = new SolidColorBrush(Colors.Red),
                Values = new LiveCharts.ChartValues<int>(prcFailAmounts)
            };

            // 차트할당
            ChtReport.Series.Clear();
            ChtReport.Series.Add(series1);
            ChtReport.Series.Add(series2);
            ChtReport.Series.Add(series3);
            // x축 좌표값을 날짜로 표시
            ChtReport.AxisX.First().Labels = list.Select(a => a.PrcDate.ToString("yyyy-MM-dd")).ToList();
        }

        // 날짜 초기화
        private void InitControls()
        {
            DtpSearchStartDate.SelectedDate = DateTime.Now.AddDays(-7); // 일주일 전 데이터 입력
            DtpSearchEndDate.SelectedDate = DateTime.Now;               // 오늘
        }
        #endregion

    }
}
