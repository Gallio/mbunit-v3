Reports Plugin
==============

This plugin provides multiple report formats for generating MbUnit reports.
Additional report formats can be added in several ways:

1. Adding or modifying XSLT stylesheets and registering them in the
   MbUnit.Plugin.Reports.plugin file as additional XsltReportFormatter
   components similar to the existing ones.
   
2. Adding new implementations of IReportFormatter to the assembly
   and registering them in the MbUnit.Plugin.Reports.plugin file
   in similar fashion to the XmlReportFormatter.
   
3. Creating a new MbUnit plugin and registering components that implement
   the IReportFormatter interface just like XmlReportFormatter.
