function y2k(number)
{ 
	return (number < 1000) ? number + 1900 : number; 
}

function __gotoCalendar()
{
	window.open("/Util/Calendars/FromTop.aspx", "Right");
}

var now = new Date();
var dd = now.getDate();
var mt = now.getMonth() + 1;
var yy = y2k(now.getYear());
var weekVal = now.getDay();

if (weekVal==0)
   msg1="������";
else if (weekVal==1)
   msg1="����һ"; 
else if (weekVal==2)
   msg1="���ڶ�"; 
else if (weekVal==3)
   msg1="������"; 
else if (weekVal==4)
   msg1="������"; 
else if (weekVal==5)
   msg1="������"; 
else if (weekVal==6)
   msg1="������"; 

document.write("<table width=70 border=0 cellspacing=1 cellpadding=1 style=\"CURSOR:default;FILTER: shadow(color=#333333,direction=135,strength=7);\"><tr bgcolor=blue><td height=40>");
document.write("<table width=100% border=0 bgcolor=black align=center><tr align=center><td>\n");
document.write('<span STYLE="FONT-FAMILY: ����; FONT-SIZE: 9pt;text-decoration: none; color: yellow">\n');
document.write(yy+"��"+mt+"��<br>");
document.write('<font color=lime><b><font face="Tahoma, Arial, Helvetica, sans-serif" size="+2">');
document.write('<a href="" onclick="__gotoCalendar(); return false;" style="COLOR:lime" title="�鿴���յ��ճ̰��š�">'+dd+"</a></font></b></font><br>");
document.write(msg1+"</span></td></tr></table></td></tr></table>");
