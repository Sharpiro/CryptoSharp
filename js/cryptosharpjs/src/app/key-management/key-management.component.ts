import { Component, OnInit, AfterViewInit, ViewChild, Inject } from '@angular/core';
import { SelectionModel } from '@angular/cdk/collections';
import { MatTableDataSource, MatPaginator, MatSort, MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { AddKeyModalComponent } from './add-key-modal/add-key-modal.component';
import { SpinnerComponent } from '../shared/spinner/spinner.component';
import * as toastr from 'toastr';
import { KeyInfo } from '.';
import { KeyManagementService } from './key-management.service';
import { SpinnerService } from '../shared/spinner/spinner.service';

// https://stackoverflow.com/questions/50438550/angular-material-extend-a-matpaginator-without-creating-a-dupe-paginator
// @Component({
//   selector: 'app-custom-paginator',
//   template: '<mat-paginator #paginator pageSize="10" [pageSizeOptions]="[5, 10, 20]" [showFirstLastButtons]="true"></mat-paginator>',
// })
// export class CustomPaginator extends MatPaginator implements OnInit {
//   @ViewChild(MatPaginator) paginator: MatPaginator;

//   async ngOnInit() {
//     console.log(this);
//     console.log(this.paginator);
//   }

//   getCurrentPageLength(): number {
//     const pageIndex = this.pageIndex
//     const pageSize = this.pageSize
//     const minIndex = pageIndex * pageSize
//     const maxIndex = pageIndex * pageSize + pageSize - 1
//     const itemLength = this.length
//     const totalPages = this.getNumberOfPages() + 1
//     if (pageIndex + 1 !== totalPages) return pageSize
//     const remainingitems = itemLength % pageSize
//     return remainingitems === 0 ? pageSize : remainingitems
//   }
// }

@Component({
  selector: 'app-custom-paginator',
  template: '<mat-paginator #paginator pageSize="10"></mat-paginator>',
})
export class CustomPaginator extends MatPaginator {
  @ViewChild(MatPaginator) paginator: MatPaginator;

  customMethod(): number {
    return 1
  }
}

@Component({
  selector: 'app-key-management',
  templateUrl: './key-management.component.html',
  styleUrls: ['./key-management.component.css']
})
export class KeyManagementComponent implements OnInit, AfterViewInit {
  displayedColumns = ['select', 'id', 'key', 'name', 'description']
  dataSource = new MatTableDataSource<KeyInfo>()
  selection = new SelectionModel<KeyInfo>(true, []);

  @ViewChild(CustomPaginator) paginator: CustomPaginator;
  // @ViewChild(CustomPaginator) paginator: CustomPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(private keyManagementService: KeyManagementService, private dialog: MatDialog,
    private spinnerService: SpinnerService) {
  }

  async ngOnInit() {
    this.spinnerService.open();
    this.dataSource.data = await this.keyManagementService.getKeyInfoList().toPromise()
    this.spinnerService.close();
  }

  async ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    // this.dataSource.paginator.page.subscribe(res => {
    //   this.selection.clear()
    // })
  }

  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const pageSize = this.dataSource.paginator.pageSize;
    const currentPageLength = -1//this.paginator.getCurrentPageLength()
    return numSelected === pageSize || numSelected == currentPageLength;
  }

  masterToggle() {
    const pageIndex = this.dataSource.paginator.pageIndex
    const pageSize = this.dataSource.paginator.pageSize
    this.isAllSelected() ?
      this.selection.clear() :
      this.dataSource.data.slice(pageIndex * pageSize, pageIndex * pageSize + pageSize).forEach(row => this.selection.select(row));
  }

  onDeleteClick() {
    const map = {}
    for (const temp of this.selection.selected) {
      map[temp.id] = true
    }
    this.dataSource.data = this.dataSource.data.filter(s => map[s.id] !== true)
    this.selection.clear();
    this.paginator.firstPage()
  }

  async onResetClick() {
    this.spinnerService.open()
    this.dataSource.data = await this.keyManagementService.getKeyInfoList().toPromise()
    this.spinnerService.close()
  }

  async onSaveClick() {
    this.spinnerService.open()
    await this.keyManagementService.setKeyInfoList(this.dataSource.data).toPromise()
    this.spinnerService.close()
  }

  onDebugClick() {
    const pageIndex = this.dataSource.paginator.pageIndex
    const pageSize = this.dataSource.paginator.pageSize
    const minIndex = pageIndex * pageSize
    const maxIndex = pageIndex * pageSize + pageSize - 1
    const itemLength = this.dataSource.paginator.length
    const totalPages = this.dataSource.paginator.getNumberOfPages() + 1
    const pageSlice = this.dataSource.data.slice(pageIndex * pageSize, pageIndex * pageSize + pageSize)
    console.log("pageIndex:", pageIndex)
    console.log("maxPageSize:", pageSize)
    console.log("min:", minIndex)
    console.log("max:", maxIndex)
    console.log("length:", itemLength)
    console.log("pages:", totalPages)
    console.log("currentPageSize:", pageSlice)
    console.log("pageSlice:", this.getCurrentPageLength())
  }

  private getCurrentPageLength(): number {
    const pageIndex = this.dataSource.paginator.pageIndex
    const pageSize = this.dataSource.paginator.pageSize
    const minIndex = pageIndex * pageSize
    const maxIndex = pageIndex * pageSize + pageSize - 1
    const itemLength = this.dataSource.paginator.length
    const totalPages = this.dataSource.paginator.getNumberOfPages() + 1
    if (pageIndex + 1 !== totalPages) return pageSize
    const remainingitems = itemLength % pageSize
    return remainingitems === 0 ? pageSize : remainingitems
  }

  onAddClick() {
    const dialogRef: MatDialogRef<AddKeyModalComponent, KeyInfo> = this.dialog.open(AddKeyModalComponent, {
      width: `${window.innerWidth * .75}px`,
      data: {},
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) return;
      if (!result.key || !result.name) {
        toastr.error("invalid row data")
        return
      }
      this.dataSource.data.push(result)
      this.dataSource.data = this.dataSource.data
      this.paginator.firstPage()
    });
  }
}