function toggle(id)
{
    var icon = document.getElementById('toggle-' + id);
    if (icon != null)
    {
        var childElement = document.getElementById(id);
        if (icon.src.indexOf('Plus.gif') != -1)
        {
            icon.src = icon.src.replace('Plus.gif', 'Minus.gif');
            if (childElement != null)
            {
                childElement.style.display = "block";
            }
        }
        else
        {
            icon.src = icon.src.replace('Minus.gif', 'Plus.gif');
            if (childElement != null)
            {
                childElement.style.display = "none";
            }
        }
    }
}
