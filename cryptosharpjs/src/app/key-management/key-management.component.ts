import { Component, OnInit, AfterViewInit, ViewChild } from '@angular/core';
import { SelectionModel } from '@angular/cdk/collections';
import { MatTableDataSource, MatPaginator, MatSort } from '@angular/material';

@Component({
  selector: 'app-key-management',
  templateUrl: './key-management.component.html',
  styleUrls: ['./key-management.component.css']
})
export class KeyManagementComponent implements OnInit, AfterViewInit {
  displayedColumns = ['select', 'id', 'key', 'name', 'description']
  dataSource = new MatTableDataSource<Entry>(ELEMENT_DATA.map((d, i) => {
    const obj: Entry = { id: i + 1, key: d.key, name: d.name, description: d.description }
    return obj
  }));
  selection = new SelectionModel<Entry>(true, []);

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor() { }

  ngOnInit() { }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.dataSource.data.forEach(row => this.selection.select(row));
  }

  deleteSelected() {
    // this.dataSource.data = []
    // console.log(this.dataSource.data)
    // console.log(ELEMENT_DATA)
    // this.selection.clear()
    // const temp = { ["temp"]: 12 }
    const map = {}
    for (const temp of this.selection.selected) {
      map[temp.id] = true
    }
    this.dataSource.data = this.dataSource.data.filter(s => map[s.id] !== true)
    // console.log(this.selection.selected[0].name = "difffffffffffffffffff")
  }
}

let ELEMENT_DATA: Entry[] = [
  { key: "aabb", name: "key1", description: "key1D" },
  { key: "aacc", name: "key2", description: "key2D" },
  { key: "aadd", name: "key2", description: "key3D" },
  { key: "aabb", name: "key1", description: "key1D" },
  { key: "aacc", name: "key2", description: "key2D" },
  { key: "aadd", name: "key2", description: "key3D" },
  { key: "aabb", name: "key1", description: "key1D" },
  { key: "aacc", name: "key2", description: "key2D" },
  { key: "aadd", name: "key2", description: "key3D" },
  { key: "aabb", name: "key1", description: "key1D" },
  { key: "aacc", name: "key2", description: "key2D" },
  { key: "aadd", name: "key2", description: "key3D" },
  { key: "aabb", name: "key1", description: "key1D" },
  { key: "aacc", name: "key2", description: "key2D" },
  { key: "aadd", name: "key2", description: "key3D" },
  { key: "aabb", name: "key1", description: "key1D" },
  { key: "aacc", name: "key2", description: "key2D" },
  { key: "aadd", name: "key2", description: "key3D" },
]

interface Entry {
  id?: number
  key: string
  name: string
  description: string
}