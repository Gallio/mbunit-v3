function toggle(id)
{
    var icon = document.getElementById('toggle' + id);
    var childList = document.getElementById('list' + id);
    if (icon.src.indexOf('Plus.gif') != -1)
    {
        icon.src = "img/Minus.gif";
        childList.style.display = "block";
    }
    else
    {
        icon.src = "img/Plus.gif";
        childList.style.display = "none";
    }
}