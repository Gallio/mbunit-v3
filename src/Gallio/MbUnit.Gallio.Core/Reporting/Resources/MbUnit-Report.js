function toggle(id)
{
    var icon = document.getElementById('toggle' + id);
    if (icon != null)
    {
        var childList = document.getElementById('list' + id);
        var childDiv = document.getElementById('executionLog' + id);
        if (icon.src.indexOf('Plus.gif') != -1)
        {
            icon.src = "img/Minus.gif";
            if (childList != null)
            {
                childList.style.display = "block";
            }
            if (childDiv != null)
            {
                childDiv.style.display = "block";
            }
        }
        else
        {
            icon.src = "img/Plus.gif";
            if (childList != null)
            {
                childList.style.display = "none";
            }
            if (childDiv != null)
            {
                childDiv.style.display = "none";
            }
        }
    }
}