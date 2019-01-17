var gAlternativeFilterNames = { 
    "Basler MED ace 2.3 MP 41 color" : "acA1920-40ucMED",
    "Basler MED ace 2.3 MP 41 mono" : "acA1920-40umMED",
    "Basler MED ace 2.3 MP 164 color" : "acA1920-155ucMED",
    "Basler MED ace 2.3 MP 164 mono" : "acA1920-155umMED",
    "Basler MED ace 5.1 MP 35 color" : "acA2440-35ucMED",
    "Basler MED ace 5.1 MP 35 mono" : "acA2440-35umMED",
    "Basler MED ace 5.1 MP 75 color" : "acA2440-75ucMED",
    "Basler MED ace 5.1 MP 75 mono" : "acA2440-75umMED",
    "Basler MED ace 5.3 MP 20 color" : "acA2500-20gcMED",
    "Basler MED ace 5.3 MP 20 mono" : "acA2500-20gmMED"
};  
var gCameraNamingScheme = [ 
    { 
        seriesid: "acA", 
        interfaceid: "g",
        colorid: "m",
        seriestag: "_ace_GigE",
        sfnctag: "_SFNC_1",
        colortag: "_Mono",
        suffix: ""
    }, 
    { 
        seriesid: "acA", 
        interfaceid: "g",
        colorid: "c",
        seriestag: "_ace_GigE",
        sfnctag: "_SFNC_1",
        colortag: "_Color",
        suffix: ""
    }, 
    { 
        seriesid: "acA", 
        interfaceid: "u",
        colorid: "m", 
        seriestag: "_ace_USB", 
        sfnctag: "_SFNC_2",
        colortag: "_Mono",
        suffix: ""
    },
    { 
        seriesid: "acA", 
        interfaceid: "u",
        colorid: "c",
        seriestag: "_ace_USB", 
        sfnctag: "_SFNC_2",
        colortag: "_Color",
        suffix: ""
    }, 
    { 
        seriesid: "acA", 
        interfaceid: "g",
        colorid: "m",
        seriestag: "_ace_GigE_MED",
        sfnctag: "_SFNC_1",
        colortag: "_Mono",
        suffix: "MED"
    }, 
    { 
        seriesid: "acA", 
        interfaceid: "g",
        colorid: "c",
        seriestag: "_ace_GigE_MED",
        sfnctag: "_SFNC_1",
        colortag: "_Color",
        suffix: "MED"
    }, 
    { 
        seriesid: "acA", 
        interfaceid: "u",
        colorid: "m", 
        seriestag: "_ace_USB_MED", 
        sfnctag: "_SFNC_2",
        colortag: "_Mono",
        suffix: "MED"
    },
    { 
        seriesid: "acA", 
        interfaceid: "u",
        colorid: "c",
        seriestag: "_ace_USB_MED", 
        sfnctag: "_SFNC_2",
        colortag: "_Color",
        suffix: "MED"
    },     
    { 
        seriesid: "daA", 
        interfaceid: "l",
        colorid: "c",
        seriestag: "_dart_LVDS",
        sfnctag: "_SFNC_2",
        colortag: "_Color",
        suffix: ""
    },
    { 
        seriesid: "daA", 
        interfaceid: "l",
        colorid: "m", 
        seriestag: "_dart_LVDS", 
        sfnctag: "_SFNC_2",
        colortag: "_Mono",
        suffix: ""
    }, 
    { 
        seriesid: "daA", 
        interfaceid: "m",
        colorid: "m",
        seriestag: "_dart_MIPI", 
        sfnctag: "_SFNC_2",
        colortag: "_Mono",
        suffix: ""
    },
    { 
        seriesid: "daA", 
        interfaceid: "m",
        colorid: "c",
        seriestag: "_dart_MIPI", 
        sfnctag: "_SFNC_2",
        colortag: "_Color",
        suffix: ""
    },
    { 
        seriesid: "daA", 
        interfaceid: "u",
        colorid: "m", 
        seriestag: "_dart_USB", 
        sfnctag: "_SFNC_2",
        colortag: "_Mono",
        suffix: ""
    }, 
    { 
        seriesid: "daA", 
        interfaceid: "u",
        colorid: "c", 
        seriestag: "_dart_USB", 
        sfnctag: "_SFNC_2",
        colortag: "_Color",
        suffix: ""
    },    
    { 
        seriesid: "puA", 
        interfaceid: "u",
        colorid: "m",
        seriestag: "_pulse_USB", 
        sfnctag: "_SFNC_2",
        colortag: "_Mono",
        suffix: ""
    },
    { 
        seriesid: "puA", 
        interfaceid: "u",
        colorid: "c",
        seriestag: "_pulse_USB", 
        sfnctag: "_SFNC_2",
        colortag: "_Color",
        suffix: ""
    }
];
var gLanguageStrings = {
    en_US: {
        showAllEnable: "Show all camera models",
        showAllDisable: "Show only your camera model",
        topicFooter: "<p>Suggestions for improving the documentation? Send us your <a id='mailFeedback'>feedback on this topic</a>.</p><p>For technical questions, please contact your <a href='https://www.baslerweb.com/en/sales/'>local distributor</a> or use the <a class='external-link' href='http://www.baslerweb.com/en/support/contact' target='_blank'>support form</a> on the Basler website.</p>"
    }    
};