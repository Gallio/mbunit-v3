function toggle(id)
{
    var icon = document.getElementById('toggle-' + id);
    if (icon != null)
    {
        var childElement = document.getElementById(id);
        if (icon.src.indexOf('Plus.gif') != -1)
        {
            icon.src = "img/Minus.gif";
            if (childElement != null)
            {
                childElement.style.display = "block";
            }
        }
        else
        {
            icon.src = "img/Plus.gif";
            if (childElement != null)
            {
                childElement.style.display = "none";
            }
        }
    }
}