import { Component, OnInit } from '@angular/core';
import { Buffer } from "buffer"

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'app';
  opened = true

  ngOnInit() { }
}
