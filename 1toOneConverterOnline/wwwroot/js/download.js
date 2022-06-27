function download(fileName, bytes) {
    var blob = new Blob([bytes], { type: "application/octet-stream" });
    var link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = fileName;
    link.click();
}
//# sourceMappingURL=download.js.map