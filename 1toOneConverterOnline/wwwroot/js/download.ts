function download(fileName: string, bytes: Uint8Array) {
    var blob = new Blob([bytes], { type: "application/octet-stream" });

    var link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = fileName;
    link.click();
}