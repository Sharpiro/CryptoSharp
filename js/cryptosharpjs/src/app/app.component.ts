import { Component, OnInit, ViewChild } from '@angular/core';
import { Buffer } from "buffer"
import { MatSidenav } from '@angular/material';
import { booleanString } from './shared/types/custom-types';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'cryptosharpjs';
  opened = true
  sidenavPropertyName = "sidenavStatus"

  @ViewChild(MatSidenav) sidenav: MatSidenav;

  ngOnInit() {
    const sidenavStatus = <booleanString>localStorage.getItem(this.sidenavPropertyName)
    if (!sidenavStatus) return
    this.opened = sidenavStatus === "true"
  }

  toggle() {
    this.opened = !this.opened
    localStorage.setItem(this.sidenavPropertyName, this.opened.toString())
  }
}
