
function downloadFile(base64Data, filename)
{
    const link = document.createElement("a");
    link.href = "data:text/json;base64," + base64Data;
    link.download = filename;
    link.click();
}