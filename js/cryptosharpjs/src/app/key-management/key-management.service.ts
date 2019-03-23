import { Injectable } from '@angular/core';
import { KeyInfo } from '.';
import { Observable } from 'rxjs';

@Injectable()
export class KeyManagementService {
  private keyInfoListKey = "keyInfoList"
  // private keyInfoList: KeyInfo[] = [
  //   { key: "aabb", name: "key1", description: "key1D" },
  //   { key: "aacc", name: "key2", description: "key2D" },
  //   { key: "aadd", name: "key2", description: "key3D" },
  //   { key: "aabb", name: "key1", description: "key1D" },
  //   { key: "aacc", name: "key2", description: "key2D" },
  //   { key: "aadd", name: "key2", description: "key3D" },
  //   { key: "aabb", name: "key1", description: "key1D" },
  //   { key: "aacc", name: "key2", description: "key2D" },
  //   { key: "aadd", name: "key2", description: "key3D" },
  //   { key: "aabb", name: "key1", description: "key1D" },
  //   { key: "aabb", name: "key1", description: "key1D" },
  //   { key: "aabb", name: "key1", description: "key1D" },
  // ]

  private keyInfoList: KeyInfo[] = []

  constructor() {
    const data = localStorage.getItem(this.keyInfoListKey)
    if (!data) return
    this.keyInfoList = JSON.parse(data)
  }

  getKeyInfoList(): Observable<KeyInfo[]> {
    return Observable.of(this.keyInfoList.map((d, i) => {
      d.id = i + 1
      return d
    })).delay(1000)
  }

  setKeyInfoList(entries: KeyInfo[]): Observable<any> {
    this.keyInfoList = entries
    localStorage.setItem(this.keyInfoListKey, JSON.stringify(entries))
    return Observable.of().delay(1000)
  }
}