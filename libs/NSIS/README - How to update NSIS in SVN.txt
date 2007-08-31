To update NSIS to a new version.

1. Download the version you want from the NSIS web site.

2. Install it to your machine.

3. Copy all files from C:\Program Files\NSIS to the libs\NSIS folder in SVN.

4. As currently configured in SVN, a good number of files have been excluded
   using the "svn:ignore" property.  This includes the NSIS menu, docs,
   examples and other bits.  We don't need most of that stuff.

   So if new files have been added to NSIS, carefully decide whether we really
   need them in SVN.  If not, then add them to the SVN ignores list before
   checking in the changes.

   Note: Certain license files must be included in SVN for compliance, do not remove them.

5. Test it by running a full release build.
 