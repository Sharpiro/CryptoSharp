import { Buffer } from "buffer"

export class FileInfo {
    public get data(): Buffer {
        return this._data;
    }
    public get fullName(): string {
        return this._fullName
    }
    public get name(): string {
        const lastDotIndex = this._fullName.lastIndexOf(".")
        return this._fullName.slice(0, lastDotIndex)
    }
    public get extension(): string {
        const lastDotIndex = this._fullName.lastIndexOf(".")
        return this._fullName.slice(lastDotIndex)
    }

    constructor(private _data: Buffer, private _fullName: string) { }
}