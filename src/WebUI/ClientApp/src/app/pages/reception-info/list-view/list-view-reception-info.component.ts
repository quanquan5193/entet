import { AfterViewInit, Component, ElementRef, OnInit, Renderer2, ViewChild } from '@angular/core';
import { NgbCalendar, NgbDate, NgbDateParserFormatter, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import * as dayjs from 'dayjs';
import { CONFIG } from 'src/app/share/constants/config.constants';
import { DATA } from 'src/app/share/constants/data.constants';
import { CloseModalComponent } from 'src/app/share/modal/close-modal/close-modal.component';
import { CommonMethodsService } from 'src/app/share/services/common-methods.service';
import { ReceptionClient, ReceptionGraphResult } from 'src/app/web-api-client';
import * as htmlToImage from 'html-to-image';
import { toPng, toJpeg, toBlob, toPixelData, toSvg } from 'html-to-image';
import { PERMISSION } from 'src/app/share/constants/permission.constants';

@Component({
  selector: 'app-list-view-reception-info-table',
  templateUrl: './list-view-reception-info.component.html',
  styleUrls: ['./list-view-reception-info.component.scss'],
})
export class ListViewReceptionInfoComponent implements OnInit, AfterViewInit {
  @ViewChild('inputArea') inputArea: ElementRef;
  @ViewChild('closeModal') private closeModalComponent: CloseModalComponent;
  public permission = PERMISSION;

  // Chart setup
  chartHeight = 0;
  textArea = DATA.DisplayChart;
  showXAxis: boolean = true;
  showYAxis: boolean = true;
  showLegend: boolean = false;
  showGridLine: boolean = true;
  showXAxisLabel: boolean = false;
  xAxisLabel: string = 'Day';
  showYAxisLabel: boolean = false;
  noBarWhenZero: boolean = false;
  animations: boolean = true;
  yAxisLabel: string = '';
  view: any[] = [2000];
  colorScheme = {
    domain: ['#f4a261', '#2a9d8f', '#2b7ccc', '#696969']
  };
  multi = [];
  isDisplayDateExport = false;
  isChartNoData = false;

  // Variable Checkbox
  showChart = true;
  showTable = false;
  isCreateSelected = true;
  isSwitchSelected = true;
  isReissuedSelected = true;
  isChangeSelected = true;
  isDiscardSelected = true;
  isPointSelected = true;
  isKidSelected = true;
  isCreateSelectedDisplayed = true;
  isSwitchSelectedDisplayed = true;
  isReissuedSelectedDisplayed = true;
  isChangeSelectedDisplayed = true;
  isDiscardSelectedDisplayed = true;
  isPointSelectedDisplayed = true;
  isKidSelectedDisplayed = true;
  oneYearSelected = false;
  oneMonthSelected = false;
  oneWeekSelected = false;
  todaySelected = false;
  isLoadingChart = false;

  // Total Record
  totalCreateNew = 0;
  totalSwitchCard = 0;
  totalKidClub = 0;
  totalOther = 0;

  // Datepicker Data
  fromDate: NgbDate | null;
  toDate: NgbDate | null;
  hoveredDate: NgbDate | null = null;
  fromDateModel: any;
  toDateModel: any;

  // Table Data
  rowData = [];
  isInitGrid = true;
  columnDefs = [
    { field: 'receptionDate', headerName: '受付日', minWidth: 150, cellRenderer: date => dayjs(date.value).format(CONFIG.DateFormat) },
    { field: 'totalCreateCards', headerName: '新規', minWidth: 60 },
    { field: 'totalSwitchCards', headerName: '切替', minWidth: 60 },
    { field: 'totalReissuedCards', headerName: '再発行', minWidth: 60 },
    { field: 'totalChangeCards', headerName: '変更', minWidth: 60 },
    { field: 'totalDiscardCards', headerName: '退会', minWidth: 60 },
    { field: 'totalPointMigration', headerName: 'P移行', minWidth: 60 },
    { field: 'totalKidClubs', headerName: 'キッズ', minWidth: 60 },
  ];
  page = 1;
  totalRecords = 0;
  recordsPerPage = 50;
  isShowButtonExport = false;

  constructor (
    private calendar: NgbCalendar,
    public formatter: NgbDateParserFormatter,
    private translate: TranslateService,
    private receptionClients: ReceptionClient,
    private common: CommonMethodsService,
  ) { }

  ngAfterViewInit() {
    this.chartHeight = this.inputArea.nativeElement.offsetHeight - 225;
    setTimeout(() => {
      const today = this.calendar.getToday();
      this.todaySelected = true;
      this.fromDate = today;
      this.toDate = today;
      this.loadDetailChart();
    }, 100);
  }

  ngOnInit() {
  }

  yAxisTickFormatting = (value: number) => {
    if (Math.floor(value) !== value) {
      return '';
    }
    return value;
  };

  xAxisTickFormatting = (value: string) => {
    // if (value && value.split('/').length == 3) {
    //   return value.split('/')[0] + '/' + value.split('/')[1];
    // }
    return value;
  };

  handleClickFilter(id: number) {
    switch (id) {
      case 1:
        this.isCreateSelectedDisplayed = !this.isCreateSelectedDisplayed;
        break;
      case 2:
        this.isSwitchSelectedDisplayed = !this.isSwitchSelectedDisplayed;
        break;
      case 3:
        this.isReissuedSelectedDisplayed = !this.isReissuedSelectedDisplayed;
        break;
      case 4:
        this.isChangeSelectedDisplayed = !this.isChangeSelectedDisplayed;
        break;
      case 5:
        this.isDiscardSelectedDisplayed = !this.isDiscardSelectedDisplayed;
        break;
      case 6:
        this.isPointSelectedDisplayed = !this.isPointSelectedDisplayed;
        break;
      case 7:
        this.isKidSelectedDisplayed = !this.isKidSelectedDisplayed;
        break;
    }
  }

  handleViewByChart() {
    if (!(this.fromDate && this.toDate)) {
      this.closeModalComponent.open(this.translate.instant('receptionInfo.mustInputFullDate'));
      return;
    }

    const fiveYearNextOneDay = this.calendar.getNext(this.fromDate, 'y', 5);
    const fiveYearNext = this.calendar.getNext(fiveYearNextOneDay, 'd', -1);

    if (this.toDate.after(fiveYearNext)) {
      this.closeModalComponent.open(this.translate.instant('receptionInfo.mustInputFullDate'));
      return;
    }

    this.bindingDataSearch();
    this.resetButtonSelectDate();
    this.showChart = true;
    this.showTable = false;
    this.loadDetailChart();
  }

  loadDetailChart() {
    this.receptionClients.getAdminCardsWithCondition(
      `${this.fromDate.year}/${this.fromDate.month}/${this.fromDate.day}`,
      `${this.toDate.year}/${this.toDate.month}/${this.toDate.day}`,
      this.isCreateSelected,
      this.isSwitchSelected,
      this.isReissuedSelected,
      this.isChangeSelected,
      this.isDiscardSelected,
      this.isPointSelected,
      this.isKidSelected
    ).subscribe(result => {
      if (result && result.listData && result.listData.length > 0) {
        this.isChartNoData = false;
        this.mapDataToGraph(result);
      } else {
        this.isChartNoData = true;
      }
    }, () => this.closeModalComponent.open(this.translate.instant('system.error.fetchDataFail')));
  }

  handleViewByDetail() {
    if (!(this.fromDate && this.toDate)) {
      this.closeModalComponent.open(this.translate.instant('receptionInfo.mustInputFullDate'));
      return;
    }

    const fiveYearNextOneDay = this.calendar.getNext(this.fromDate, 'y', 5);
    const fiveYearNext = this.calendar.getNext(fiveYearNextOneDay, 'd', -1);

    if (this.toDate.after(fiveYearNext)) {
      this.closeModalComponent.open(this.translate.instant('receptionInfo.mustInputFullDate'));
      return;
    }

    this.bindingDataSearch();
    this.resetButtonSelectDate();
    this.showChart = false;
    this.showTable = true;
    this.page = 1;
    this.loadDetailTable();
  }

  loadDetailTable() {
    this.receptionClients.getAdminCardsWithPagination(
      `${this.fromDate.year}/${this.fromDate.month}/${this.fromDate.day}`,
      `${this.toDate.year}/${this.toDate.month}/${this.toDate.day}`,
      this.isCreateSelected,
      this.isSwitchSelected,
      this.isReissuedSelected,
      this.isChangeSelected,
      this.isDiscardSelected,
      this.isPointSelected,
      this.isKidSelected,
      this.recordsPerPage,
      this.page
    ).subscribe(result => {
      this.totalRecords = result.totalCount;
      this.rowData = result.items;
      this.isShowButtonExport = this.rowData.length > 0;
      this.isInitGrid = false;
    }, () => this.closeModalComponent.open(this.translate.instant('system.error.fetchDataFail')));
  }

  onPageChangeEvent(event) {
    this.page = event;
    this.loadDetailTable();
  }

  handleClickSearch(num: number) {
    this.resetButtonSelectDate();
    const today = this.calendar.getToday();

    switch (num) {
      case 1:
        this.oneYearSelected = true;
        this.fromDate = this.calendar.getNext(today, 'y', -1);
        this.toDate = this.calendar.getNext(today, 'd', -1);
        break;
      case 2:
        this.oneMonthSelected = true;
        this.fromDate = this.calendar.getNext(today, 'm', -1);
        this.toDate = this.calendar.getNext(today, 'd', -1);
        break;
      case 3:
        this.oneWeekSelected = true;
        this.fromDate = this.calendar.getNext(today, 'd', -7);
        this.toDate = this.calendar.getNext(today, 'd', -1);
        break;
      case 4:
        this.todaySelected = true;
        this.fromDate = today;
        this.toDate = today;
        break;
    }

    this.loadDetailChart();
  }

  resetButtonSelectDate() {
    this.oneYearSelected = false;
    this.oneMonthSelected = false;
    this.oneWeekSelected = false;
    this.todaySelected = false;
  }

  handleExportGraph() {
    this.isLoadingChart = true;
    setTimeout(() => {
      const node = document.getElementById('chart-view-export');
      htmlToImage.toBlob(node, { backgroundColor: "#ffffff", filter: this.filter })
        .then(blob => {
          this.sentImageToServer(blob);
        });
    }, 100);
  }

  sentImageToServer(data) {
    const param = {
      data: data,
      fileName: 'chart'
    };
    this.receptionClients.getAdminReceptionsExportGraph(param).subscribe(result => {
      this.saveFileToClient(result, '受付状況グラフ＿');
    }, () => this.closeModalComponent.open(this.translate.instant('system.error.fetchDataFail')));
    this.isLoadingChart = false;
  }

  filter(node: HTMLElement) {
    return (node.id !== 'button-date');
  }

  handleExportTable(event) {
    this.receptionClients.getAdminReceptionsExportTable(
      `${this.fromDate.year}/${this.fromDate.month}/${this.fromDate.day}`,
      `${this.toDate.year}/${this.toDate.month}/${this.toDate.day}`,
      this.isCreateSelected,
      this.isSwitchSelected,
      this.isReissuedSelected,
      this.isChangeSelected,
      this.isDiscardSelected,
      this.isPointSelected,
      this.isKidSelected
    ).subscribe(result => {
      this.saveFileToClient(result, '受付状況集計＿');
    }, () => this.closeModalComponent.open(this.translate.instant('system.error.fetchDataFail')));
  }

  saveFileToClient(result, fileSaveName) {
    const blobContent = result.data;
    const extension = result.fileName.substring(result.fileName.lastIndexOf('.') + 1);
    const currentDate = dayjs().format('YYYYMMDD＿HHmmss');
    const fileName = `${fileSaveName}${currentDate}.${extension}`;
    if (window.navigator && window.navigator.msSaveOrOpenBlob) {
      const newBlob = new Blob([blobContent], {
        type: this.common.getMimeType(this.getFileType(fileName)),
      });
      window.navigator.msSaveOrOpenBlob(newBlob, fileName);
      return;
    } else {
      const newFile = new File([blobContent], fileName);
      const data = window.URL.createObjectURL(newFile);
      const link = document.createElement("a");
      link.href = data;
      link.download = fileName;
      link.click();
    }
  }

  getFileType(attachment) {
    return this.split(attachment.fileType, ".")[1];
  }

  split(val, separator) {
    const separatorNew = separator.replace(/([^\w\s])/g, "\\$1");
    return val.split(new RegExp(separatorNew));
  }

  bindingDataSearch() {
    this.isCreateSelected = this.isCreateSelectedDisplayed;
    this.isSwitchSelected = this.isSwitchSelectedDisplayed;
    this.isReissuedSelected = this.isReissuedSelectedDisplayed;
    this.isChangeSelected = this.isChangeSelectedDisplayed;
    this.isDiscardSelected = this.isDiscardSelectedDisplayed;
    this.isPointSelected = this.isPointSelectedDisplayed;
    this.isKidSelected = this.isKidSelectedDisplayed;
  }

  mapDataToGraph(res: ReceptionGraphResult) {
    const totalNumber = res.total;
    const totalValue = res.listData;
    this.totalCreateNew = totalNumber.totalCreateCards;
    this.totalSwitchCard = totalNumber.totalSwitchCards;
    this.totalKidClub = totalNumber.totalKidClubs;
    this.totalOther = totalNumber.totalOther;
    let listData = [];
    this.isDisplayDateExport = totalValue[0].isDisplayMonth ? totalValue[0].isDisplayMonth : this.calendar.getNext(this.fromDate, 'm', 1).after(this.toDate);
    totalValue.forEach((element) => {
      const item = {
        "name": element.isDisplayMonth ? dayjs(element.receptionDate).format('YYYY/MM') : dayjs(element.receptionDate).format('YYYY/MM/DD'),
        "series": [
          {
            "name": this.textArea.CreateCard,
            "value": element.totalCreateCards
          },
          {
            "name": this.textArea.SwitchCard,
            "value": element.totalSwitchCards
          },
          {
            "name": this.textArea.KidClub,
            "value": element.totalKidClubs
          },

          {
            "name": this.textArea.Other,
            "value": element.totalOther
          }
        ]
      };
      listData.push(item);
    });
    this.multi = listData;
    this.view = [this.calculateChartWidth(listData.length), this.chartHeight];
  }

  calculateChartWidth(width) {
    if (width == 1) return 110;
    if (width < 4) return width * 70;
    if (width < 10) return ((width + 1) * 30) + 140;
    return ((width + 1) * 50) + 140;
  }

  calculateChartExportWidth(width) {
    if (width == 1) return 110;
    if (width < 4) return width * 70;
    if (width < 10) return ((width + 1) * 30) + 140;
    if (width < 15) return ((width + 1) * 40) + 140;
    return 1100;
  }

  onDateSelection(date: NgbDate) {
    if (!this.fromDate && !this.toDate) {
      this.fromDate = date;
    } else if (this.fromDate && !this.toDate && date && (date.after(this.fromDate) || date.equals(this.fromDate))) {
      this.toDate = date;
    } else {
      this.toDate = null;
      this.fromDate = date;
    }
  }

  validateFromDate(date) {
    const parsed = this.formatter.parse(date.target.value);
    if (parsed == null) {
      this.fromDateModel = null;
      this.fromDate = null;
      return;
    }
    if (parsed.year < 1900 || parsed.year > 2100) {
      this.fromDateModel = null;
      this.fromDate = null;
      return;
    };
    if (parsed.day == null) {
      this.fromDateModel = null;
      this.fromDate = null;
      return;
    }
    const numbers = date.target.value.trim().split('/');
    if (!(+numbers[2])) {
      this.fromDateModel = null;
      this.fromDate = null;
      return;
    }
  }

  validateToDate(date) {
    const parsed = this.formatter.parse(date.target.value);
    if (parsed == null) {
      this.toDateModel = null;
      this.toDate = null;
      return;
    }
    if (parsed.year < 1900 || parsed.year > 2100) {
      this.toDateModel = null;
      this.toDate = null;
      return;
    };
    if (parsed.day == null) {
      this.toDateModel = null;
      this.toDate = null;
      return;
    }
    const numbers = date.target.value.trim().split('/');
    if (!(+numbers[2])) {
      this.toDateModel = null;
      this.toDate = null;
      return;
    }
  }

  formatDate(date: NgbDateStruct): string {
    if (date) return '' + date.year + '/' + (date.month <= 9 ? '0' + date.month : date.month) + '/' + (date.day <= 9 ? '0' + date.day : date.day);
    return null;
  }

  isRange(date: NgbDate) {
    return date.equals(this.fromDate) || (this.toDate && date.equals(this.toDate)) || this.isInside(date) || this.isHovered(date);
  }

  isInside(date: NgbDate) {
    return this.toDate && date.after(this.fromDate) && date.before(this.toDate);
  }

  isHovered(date: NgbDate) {
    return this.fromDate && !this.toDate && this.hoveredDate && date.after(this.fromDate) && date.before(this.hoveredDate);
  }

  validateInput(currentValue: NgbDate | null, input: string): NgbDate | null {
    const parsed = this.formatter.parse(input);
    return parsed && this.calendar.isValid(NgbDate.from(parsed)) ? NgbDate.from(parsed) : currentValue;
  }
}
