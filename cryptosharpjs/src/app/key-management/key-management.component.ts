import { Component, OnInit, AfterViewInit, ViewChild, Inject } from '@angular/core';
import { SelectionModel } from '@angular/cdk/collections';
import { MatTableDataSource, MatPaginator, MatSort, MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { AddKeyModalComponent } from './add-key-modal/add-key-modal.component';
import { SpinnerComponent } from '../shared/spinner/spinner.component';
import * as toastr from 'toastr';
import { KeyInfo } from '.';
import { KeyManagementService } from './key-management.service';
import { SpinnerService } from '../shared/spinner/spinner.service';

@Component({
  selector: 'app-key-management',
  templateUrl: './key-management.component.html',
  styleUrls: ['./key-management.component.css']
})
export class KeyManagementComponent implements OnInit, AfterViewInit {
  displayedColumns = ['select', 'id', 'key', 'name', 'description']
  dataSource = new MatTableDataSource<KeyInfo>()
  selection = new SelectionModel<KeyInfo>(true, []);

  @ViewChild(MatPaginator) paginator: MatPaginator;
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
    this.dataSource.paginator.page.subscribe(res => {
      this.selection.clear()
    })
    this.dataSource.sort = this.sort;
  }

  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const pageSize = this.dataSource.paginator.pageSize;
    return numSelected === pageSize;
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
    const temp = await this.keyManagementService.setKeyInfoList(this.dataSource.data).toPromise()
    console.log("done")
  }

  onDebugClick() {
    const pageIndex = this.dataSource.paginator.pageIndex
    const pageSize = this.dataSource.paginator.pageSize
    console.log(pageIndex * pageSize)
    console.log(pageIndex * pageSize + pageSize)
    console.log(this.dataSource.data.slice(pageIndex * pageSize, pageIndex * pageSize + pageSize))
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