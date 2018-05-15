import { Buffer } from "buffer"

export function downloadFile(data: Buffer, fileName: string, type = "application/octet-stream") {
    const anchorElement = window.document.createElement('a')
    const blob = new Blob([data], { type: type })
    anchorElement.href = window.URL.createObjectURL(blob)
    anchorElement.download = fileName
    anchorElement.click()
}