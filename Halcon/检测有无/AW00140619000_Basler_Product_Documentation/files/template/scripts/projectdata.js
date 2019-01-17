// Publish project specific data
(function() {
rh = window.rh;
model = rh.model;

rh.consts('DEFAULT_TOPIC', encodeURI("#en/basler_product_documentation.htm".substring(1)));
rh.consts('HOME_FILEPATH', encodeURI("index.htm"));
rh.consts('START_FILEPATH', encodeURI('index.htm'));
rh.consts('HELP_ID', '001447B8-012F-4A66-A45D-E1DAB644646F' || 'preview');
rh.consts('LNG_STOP_WORDS', ["a", "about", "after", "against", "all", "also", "among", "an", "and", "are", "as", "at", "be", "became", "because", "been", "between", "but", "by", "can", "come", "do", "during", "each", "early", "for", "form", "found", "from", "had", "has", "have", "he", "her", "his", "however", "in", "include", "into", "is", "it", "its", "late", "later", "made", "many", "may", "me", "med", "more", "most", "near", "no", "non", "not", "of", "on", "only", "or", "other", "over", "several", "she", "some", "such", "than", "that", "the", "their", "then", "there", "these", "they", "this", "through", "to", "under", "until", "use", "was", "we", "were", "when", "where", "which", "who", "with", "you"]);
rh.consts('LNG_SUBSTR_SEARCH', 1);

model.publish(rh.consts('KEY_DIR'), "ltr");
model.publish(rh.consts('KEY_LNG_NAME'), "en_US");
model.publish(rh.consts('KEY_LNG'), {"TutorialText3":"Here, you can access basic information about the camera model selected.","SearchResultsPerScreen":"Search results per page","Reset":"Reset","Help":"Show tutorial","SyncToc":"SyncToc","HomeButton":"Home","WebSearchButton":"WebSearch","TutorialText1":"<p><strong>Information in this topic is tailored to a specific camera model.</strong></p><p>Default: acA2500-14gm.</p><p>You can change the camera model at any time.</p><p>To find out more, click <strong>Next</strong>.</p>","HighlightSearchResults":"Highlight search results","GlossaryFilterTerms":"Filter Terms","ApplyTip":"Apply","WebSearch":"WebSearch","Show":"Show","TOCFooterLinks":"<a href='en/privacy_statement.htm'>Privacy</a>&thinsp;/&thinsp;<a href='en/datenschutzerklaerung.htm'>Datenschutz</a>&ensp;•&ensp;<a href='en/licensing_information.htm'>Licensing</a>&ensp;•&ensp;<a href='en/legal_info.htm'>Legal Info</a>","ShowAll":"All","ReleaseDate":"Release date:","EnableAndSearch":"Only display topics containing all search words","Next":"Next","Print":"Print","PreviousLabel":"Previous","NoScriptErrorMsg":"Enable JavaScript support in the browser to view this page.","CopyLinkMessage":"A link to the current topic, including camera selection, has been copied to the clipboard.","Search":"Search","Hide":"Hide","Contents":"Contents","ShowHide":"Show/Hide","Canceled":"Canceled","Loading":"Loading...","EndOfResults":"End of search results.","SidebarToggleTip":"Expand/Collapse","Share":"Copy link to clipboard","Logo":"Logo","FilterDescription4":"(Internal) Indicates whether the camera is equipped with a monochrome or a color image sensor.","FilterDescription3":"(Internal) Indicates the camera interface.","FilterDescription2":"(Internal) Indicates the version of the Standard Feature Naming Convention.","FilterDescription1":"Select your camera model to display camera-specific information throughout the help.<br /><br /><b>Tip:</b> In the select box, enter any part of your camera model's name, e.g., 'acA' or '1600'.","CookieInfoText":"<span>This site uses cookies to enhance your experience. By continuing to visit this site you agree to our <a href='en/privacy_statement.htm'>use of cookies</a>.<a href='#' id='cookiebtn'>OK</a></span>","ContentFilterChanged":"Content filter is changed, search again","Logo/Author":"Powered By","PrototypeText":"<p><strong>This camera is a design-in sample.</strong></p><p>All information given is subject to change.</p>","JS_alert_LoadXmlFailed":"Failed to load XML file","ICPLicense":"ICP license number:","ChangeDeviceText":"Change camera model","VersionNumber":"19","Searching":"Searching...","SearchTitle":"Search","Disabled Next":">>","SelectedDeviceText":"Your camera model:","JS_alert_InitDatabaseFailed":"Failed to initialize database","Cancel":"Cancel","UnknownError":"Unknown error","ResultsFoundText":"%1 result(s) found for %2","Seperate":"|","Index":"Index","TopicsNotFound":"No results found","SearchPageTitle":"Search Results","Glossary":"Glossary","Filter":"Filter","Version":"Version:","TutorialText4":"Here, you can switch to the table of contents.","TableOfContents":"Table of Contents","OfflineVersion":"Offline Version","NextLabel":"Next","HideAll":"Hide All","Disabled Prev":"<<","TutorialText5":"That's it! You can always restart the tutorial by clicking here.","SelectedDeviceTextMobile":"Camera:","SearchOptions":"Search Options","Back":"Back","Prev":"Previous","OpenLinkInNewTab":"Open in new tab","WarningText":"<p><strong>The information in this topic does not apply to your camera model.</strong></p><p>Click 'Close' or anywhere outside this dialog to view anyway.</p><p>Click 'Choose camera' to select a different camera model.</p>","TutorialText2":"<p>Here, you can change the camera model.</p><p><strong>All model-specific information will be tailored to the camera model selected.</strong></p>","ToTopTip":"Go to top","NavTip":"Menu","JS_alert_InvalidExpression_1":"Error while searching. Please try again.","IndexFilterKewords":"Filter Keywords","IeCompatibilityErrorMsg":"This page cannot be viewed in Internet Explorer 8 or earlier version."});

model.publish(rh.consts('KEY_HEADER_DEFAULT_TITLE_COLOR'), "#ffffff");
model.publish(rh.consts('KEY_HEADER_DEFAULT_BACKGROUND_COLOR'), "#025172");
model.publish(rh.consts('KEY_LAYOUT_DEFAULT_FONT_FAMILY'), "\"Trebuchet MS\", Arial, sans-serif");

model.publish(rh.consts('KEY_HEADER_TITLE'), "Product Documentation");
model.publish(rh.consts('KEY_HEADER_TITLE_COLOR'), "#ffffff");
model.publish(rh.consts('KEY_HEADER_BACKGROUND_COLOR'), "#13477d");
model.publish(rh.consts('KEY_HEADER_LOGO_PATH'), "template/Basler-Screen-Layout/logo.png");
model.publish(rh.consts('KEY_LAYOUT_FONT_FAMILY'), "Roboto, Arial, sans-serif");
model.publish(rh.consts('KEY_HEADER_HTML'), "<div class='topic-header' onClick='rh._.goToFullLayout()'>\
  <div class='logo'>\
    <img src='#{logo}' />\
  </div>\
  <div class='nav'>\
    <div class='title' title='#{title}'>\
      <span>#{title}</span>\
    </div>\
    <div class='gotohome' title='#{tooltip}'>\
      <span>#{label}</span>\
    </div></div>\
  </div>\
<div class='topic-header-shadow'></div>\
");
model.publish(rh.consts('KEY_HEADER_CSS'), ".topic-header { background-color: #{background-color}; color: #{color}; width: calc(100%); height: 3em; position: fixed; left: 0; top: 0; font-family: #{font-family}; display: table; box-sizing: border-box; }\
.topic-header-shadow { height: 3em; width: 100%; }\
.logo { cursor: pointer; padding: 0.2em; text-align: center; display: table-cell; vertical-align: middle; }\
.logo img { width: 1.875em; display: block; }\
.nav { width: 100%; display: table-cell; }\
.title { width: 40%; height: 100%; float: left; line-height: 3em; cursor: pointer; }\
.gotohome { width: 60%; float: left; text-align: right; height: 100%; line-height: 3em; cursor: pointer; }\
.title span, .gotohome span { padding: 0em 1em; white-space: nowrap; text-overflow: ellipsis; overflow: hidden; display: block; }");

})();