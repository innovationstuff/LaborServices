/// <reference path="XrmPage-vsdoc.js" />
/// <reference path="sdk.metadata.js" />

/**
* http://www.openjs.com/scripts/events/keyboard_shortcuts/
* Version : 1.00.A
* By Binny V A
* License : BSD
*/

/*
    // Call an Action
    Process.callAction("mag_Retrieve",
    [{
        key: "Target",
        type: Process.Type.EntityReference,
        value: new Process.EntityReference("account", Xrm.Page.data.entity.getId())
    },
    {
        key: "ColumnSet",
        type: Process.Type.String,
        value: "name, statuscode"
    }],
    function (params) {
        // Success
        alert("Name = " + params["Entity"].get("name") + "\n" +
              "Status = " + params["Entity"].formattedValues["statuscode"]);
    },
    function (e, t) {
        // Error
        alert(e);

        // Write the trace log to the dev console
        if (window.console && console.error) {
            console.error(e + "\n" + t);
        }
    });

    // Call a Workflow
    Process.callWorkflow("4AB26754-3F2F-4B1D-9EC7-F8932331567A", Xrm.Page.data.entity.getId(),
        function () {
            alert("Workflow executed successfully");
        },
        function () {
            alert("Error executing workflow");
        });

    // Call a Dialog
    Process.callDialog("C50B3473-F346-429F-8AC7-17CCB1CA45BC", "contact", Xrm.Page.data.entity.getId(), 
        function () { 
            Xrm.Page.data.refresh(); 
        });
*/


var Process = Process || {};

// Supported Action input parameter types
Process.Type = {
    Bool: "c:boolean",
    Float: "c:double", // Not a typo
    Decimal: "c:decimal",
    Int: "c:int",
    String: "c:string",
    DateTime: "c:dateTime",
    Guid: "c:guid",
    EntityReference: "a:EntityReference",
    OptionSet: "a:OptionSetValue",
    Money: "a:Money",
    Entity: "a:Entity",
    EntityCollection: "a:EntityCollection"
}

// inputParams: Array of parameters to pass to the Action. Each param object should contain key, value, and type.
// successCallback: Function accepting 1 argument, which is an array of output params. Access values like: params["key"]
// errorCallback: Function accepting 1 argument, which is the string error message. Can be null.
// Unless the Action is global, you must specify a 'Target' input parameter as EntityReference
// actionName is required
Process.callAction = function (actionName, inputParams, successCallback, errorCallback, url) {
    var ns = {
        "": "http://schemas.microsoft.com/xrm/2011/Contracts/Services",
        ":s": "http://schemas.xmlsoap.org/soap/envelope/",
        ":a": "http://schemas.microsoft.com/xrm/2011/Contracts",
        ":i": "http://www.w3.org/2001/XMLSchema-instance",
        ":b": "http://schemas.datacontract.org/2004/07/System.Collections.Generic",
        ":c": "http://www.w3.org/2001/XMLSchema",
        ":d": "http://schemas.microsoft.com/xrm/2011/Contracts/Services",
        ":e": "http://schemas.microsoft.com/2003/10/Serialization/",
        ":f": "http://schemas.microsoft.com/2003/10/Serialization/Arrays",
        ":g": "http://schemas.microsoft.com/crm/2011/Contracts",
        ":h": "http://schemas.microsoft.com/xrm/2011/Metadata",
        ":j": "http://schemas.microsoft.com/xrm/2011/Metadata/Query",
        ":k": "http://schemas.microsoft.com/xrm/2013/Metadata",
        ":l": "http://schemas.microsoft.com/xrm/2012/Contracts",
        //":c": "http://schemas.microsoft.com/2003/10/Serialization/" // Conflicting namespace for guid... hardcoding in the _getXmlValue bit
    };

    var requestXml = "<s:Envelope";

    // Add all the namespaces
    for (var i in ns) {
        requestXml += " xmlns" + i + "='" + ns[i] + "'";
    }

    requestXml += ">" +
        "<s:Body>" +
        "<Execute>" +
        "<request>";

    if (inputParams != null && inputParams.length > 0) {
        requestXml += "<a:Parameters>";

        // Add each input param
        for (var i = 0; i < inputParams.length; i++) {
            var param = inputParams[i];

            var value = Process._getXmlValue(param.key, param.type, param.value);

            requestXml += value;
        }

        requestXml += "</a:Parameters>";
    }
    else {
        requestXml += "<a:Parameters />";
    }

    requestXml += "<a:RequestId i:nil='true' />" +
        "<a:RequestName>" + actionName + "</a:RequestName>" +
        "</request>" +
        "</Execute>" +
        "</s:Body>" +
        "</s:Envelope>";

    Process._callActionBase(requestXml, successCallback, errorCallback, url);
}

// Runs the specified workflow for a particular record
// successCallback and errorCallback accept no arguments
// workflowId, and recordId are required
Process.callWorkflow = function (workflowId, recordId, successCallback, errorCallback, url) {
    if (url == null) {
        url = Xrm.Page.context.getClientUrl();
    }

    var request = "<s:Envelope xmlns:s='http://schemas.xmlsoap.org/soap/envelope/'>" +
        "<s:Body>" +
        "<Execute xmlns='http://schemas.microsoft.com/xrm/2011/Contracts/Services' xmlns:i='http://www.w3.org/2001/XMLSchema-instance'>" +
        "<request i:type='b:ExecuteWorkflowRequest' xmlns:a='http://schemas.microsoft.com/xrm/2011/Contracts' xmlns:b='http://schemas.microsoft.com/crm/2011/Contracts'>" +
        "<a:Parameters xmlns:c='http://schemas.datacontract.org/2004/07/System.Collections.Generic'>" +
        "<a:KeyValuePairOfstringanyType>" +
        "<c:key>EntityId</c:key>" +
        "<c:value i:type='d:guid' xmlns:d='http://schemas.microsoft.com/2003/10/Serialization/'>" + recordId + "</c:value>" +
        "</a:KeyValuePairOfstringanyType>" +
        "<a:KeyValuePairOfstringanyType>" +
        "<c:key>WorkflowId</c:key>" +
        "<c:value i:type='d:guid' xmlns:d='http://schemas.microsoft.com/2003/10/Serialization/'>" + workflowId + "</c:value>" +
        "</a:KeyValuePairOfstringanyType>" +
        "</a:Parameters>" +
        "<a:RequestId i:nil='true' />" +
        "<a:RequestName>ExecuteWorkflow</a:RequestName>" +
        "</request>" +
        "</Execute>" +
        "</s:Body>" +
        "</s:Envelope>";

    var req = new XMLHttpRequest();
    req.open("POST", url + "/XRMServices/2011/Organization.svc/web", true);

    req.setRequestHeader("Accept", "application/xml, text/xml, */*");
    req.setRequestHeader("Content-Type", "text/xml; charset=utf-8");
    req.setRequestHeader("SOAPAction", "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Execute");
    req.onreadystatechange = function () {
        if (req.readyState == 4) {
            if (req.status == 200) {
                if (successCallback) {
                    successCallback();
                }
            }
            else {
                if (errorCallback) {
                    errorCallback();
                }
            }
        }
    };

    req.send(request);
}

// Pops open the specified dialog process for a particular record
// dialogId, entityName, and recordId are required
// callback fires even if the dialog is closed/cancelled
Process.callDialog = function (dialogId, entityName, recordId, callback, url) {
    tryShowDialog("/cs/dialog/rundialog.aspx?DialogId=%7b" + dialogId + "%7d&EntityName=" + entityName + "&ObjectId=" + recordId, 600, 400, callback, url);

    // Function copied from Alert.js v1.0 https://alertjs.codeplex.com
    function tryShowDialog(url, width, height, callback, baseUrl) {
        width = width || Alert._dialogWidth;
        height = height || Alert._dialogHeight;

        var isOpened = false;

        try {
            // Web (IE, Chrome, FireFox)
            var Mscrm = Mscrm && Mscrm.CrmDialog && Mscrm.CrmUri && Mscrm.CrmUri.create ? Mscrm : parent.Mscrm;
            if (Mscrm && Mscrm.CrmDialog && Mscrm.CrmUri && Mscrm.CrmUri.create) {
                // Use CRM light-box (unsupported)
                var crmUrl = Mscrm.CrmUri.create(url);
                var dialogwindow = new Mscrm.CrmDialog(crmUrl, window, width, height);

                // Allows for opening non-webresources (e.g. dialog processes) - CRM messes up when it's not a web resource (unsupported)
                if (!crmUrl.get_isWebResource()) {
                    crmUrl.get_isWebResource = function () { return true; }
                }

                // Open the lightbox
                dialogwindow.show();
                isOpened = true;

                // Make sure when the dialog is closed, the callback is fired
                // This part's all pretty unsupported, hence the try-catches
                // If you can avoid using a callback, best not to use one
                if (callback) {
                    try {
                        // Get the lightbox iframe (unsupported)
                        var $frame = parent.$("#InlineDialog_Iframe");
                        if ($frame.length == 0) { $frame = parent.parent.$("#InlineDialog_Iframe"); }
                        $frame.load(function () {
                            try {
                                // Override the CRM closeWindow function (unsupported)
                                var frameDoc = $frame[0].contentWindow;
                                var closeEvt = frameDoc.closeWindow; // OOTB close function
                                frameDoc.closeWindow = function () {
                                    // Bypasses onunload event on dialogs to prevent "are you sure..." (unsupported - doesn't work with 2015 SP1)
                                    try { frameDoc.Mscrm.GlobalVars.$B = false; } catch (e) { }

                                    // Fire the callback and close
                                    callback();
                                    try { closeEvt(); } catch (e) { }
                                }
                            } catch (e) { }
                        });
                    } catch (e) { }
                }
            }
        } catch (e) { }

        try {
            // Outlook
            if (!isOpened && window.showModalDialog) {
                // If lightbox fails, use window.showModalDialog
                baseUrl = baseUrl || Xrm.Page.context.getClientUrl();
                var params = "dialogWidth:" + width + "px; dialogHeight:" + height + "px; status:no; scroll:no; help:no; resizable:yes";

                window.showModalDialog(baseUrl + url, window, params);
                if (callback) {
                    callback();
                }

                isOpened = true;
            }
        } catch (e) { }

        return isOpened;
    }
}

Process._emptyGuid = "00000000-0000-0000-0000-000000000000";

// This can be used to execute custom requests if needed - useful for me testing the SOAP :)
Process._callActionBase = function (requestXml, successCallback, errorCallback, url) {
    if (url == null) {
        url = Xrm.Page.context.getClientUrl();
    }

    var req = new XMLHttpRequest();
    req.open("POST", url + "/XRMServices/2011/Organization.svc/web", true);
    req.setRequestHeader("Accept", "application/xml, text/xml, */*");
    req.setRequestHeader("Content-Type", "text/xml; charset=utf-8");
    req.setRequestHeader("SOAPAction", "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Execute");

    req.onreadystatechange = function () {
        if (req.readyState == 4) {
            if (req.status == 200) {
                // If there's no successCallback we don't need to check the outputParams
                if (successCallback) {
                    // Yucky but don't want to risk there being multiple 'Results' nodes or something
                    var resultsNode = req.responseXML.childNodes[0].childNodes[0].childNodes[0].childNodes[0].childNodes[1]; // <a:Results>

                    // Action completed successfully - get output params
                    var responseParams = Process._getChildNodes(resultsNode, "a:KeyValuePairOfstringanyType");

                    var outputParams = {};
                    for (i = 0; i < responseParams.length; i++) {
                        var attrNameNode = Process._getChildNode(responseParams[i], "b:key");
                        var attrValueNode = Process._getChildNode(responseParams[i], "b:value");

                        var attributeName = Process._getNodeTextValue(attrNameNode);
                        var attributeValue = Process._getValue(attrValueNode);

                        // v1.0 - Deprecated method using key/value pair and standard array
                        //outputParams.push({ key: attributeName, value: attributeValue.value });

                        // v2.0 - Allows accessing output params directly: outputParams["Target"].attributes["new_fieldname"];
                        outputParams[attributeName] = attributeValue.value;

                        /*
                        RETURN TYPES:
                            DateTime = Users local time (JavaScript date)
                            bool = true or false (JavaScript boolean)
                            OptionSet, int, decimal, float, etc = 1 (JavaScript number)
                            guid = string
                            EntityReference = { id: "guid", name: "name", entityType: "account" }
                            Entity = { logicalName: "account", id: "guid", attributes: {}, formattedValues: {} }
                            EntityCollection = [{ logicalName: "account", id: "guid", attributes: {}, formattedValues: {} }]
    
                        Attributes for entity accessed like: entity.attributes["new_fieldname"].value
                        For entityreference: entity.attributes["new_fieldname"].value.id
                        Make sure attributes["new_fieldname"] is not null before using .value
                        Or use the extension method entity.get("new_fieldname") to get the .value
                        Also use entity.formattedValues["new_fieldname"] to get the string value of optionsetvalues, bools, moneys, etc
                        */
                    }

                    // Make sure the callback accepts exactly 1 argument - use dynamic function if you want more
                    successCallback(outputParams);
                }
            }
            else {
                // Error has occured, action failed
                if (errorCallback) {
                    var message = null;
                    var traceText = null;
                    try {
                        message = Process._getNodeTextValueNotNull(req.responseXML.getElementsByTagName("Message"));
                        traceText = Process._getNodeTextValueNotNull(req.responseXML.getElementsByTagName("TraceText"));
                    } catch (e) { }
                    if (message == null) { message = "Error executing Action. Check input parameters or contact your CRM Administrator"; }
                    errorCallback(message, traceText);
                }
            }
        }
    };

    req.send(requestXml);
}

// Get only the immediate child nodes for a specific tag, otherwise entitycollections etc mess it up
Process._getChildNodes = function (node, childNodesName) {
    var childNodes = [];

    for (var i = 0; i < node.childNodes.length; i++) {
        if (node.childNodes[i].tagName == childNodesName) {
            childNodes.push(node.childNodes[i]);
        }
    }

    // Chrome uses just 'Results' instead of 'a:Results' etc
    if (childNodes.length == 0 && childNodesName.indexOf(":") !== -1) {
        childNodes = Process._getChildNodes(node, childNodesName.substring(childNodesName.indexOf(":") + 1));
    }

    return childNodes;
}

// Get a single child node for a specific tag
Process._getChildNode = function (node, childNodeName) {
    var nodes = Process._getChildNodes(node, childNodeName);

    if (nodes != null && nodes.length > 0) { return nodes[0]; }
    else { return null; }
}

// Gets the first not null value from a collection of nodes
Process._getNodeTextValueNotNull = function (nodes) {
    var value = "";

    for (var i = 0; i < nodes.length; i++) {
        if (value === "") {
            value = Process._getNodeTextValue(nodes[i]);
        }
    }

    return value;
}

// Gets the string value of the XML node
Process._getNodeTextValue = function (node) {
    if (node != null) {
        var textNode = node.firstChild;
        if (textNode != null) {
            return textNode.textContent || textNode.nodeValue || textNode.data || textNode.text;
        }
    }

    return "";
}

// Gets the value of a parameter based on its type, can be recursive for entities
Process._getValue = function (node) {
    var value = null;
    var type = null;

    if (node != null) {
        type = node.getAttribute("i:type") || node.getAttribute("type");

        // If the parameter/attribute is null, there won't be a type either
        if (type != null) {
            // Get the part after the ':' (since Chrome doesn't have the ':')
            var valueType = type.substring(type.indexOf(":") + 1).toLowerCase();

            if (valueType == "entityreference") {
                // Gets the lookup object
                var attrValueIdNode = Process._getChildNode(node, "a:Id");
                var attrValueEntityNode = Process._getChildNode(node, "a:LogicalName");
                var attrValueNameNode = Process._getChildNode(node, "a:Name");

                var lookupId = Process._getNodeTextValue(attrValueIdNode);
                var lookupName = Process._getNodeTextValue(attrValueNameNode);
                var lookupEntity = Process._getNodeTextValue(attrValueEntityNode);

                value = new Process.EntityReference(lookupEntity, lookupId, lookupName);
            }
            else if (valueType == "entity") {
                // Gets the entity data, and all attributes
                value = Process._getEntityData(node);
            }
            else if (valueType == "entitycollection") {
                // Loop through each entity, returns each entity, and all attributes
                var entitiesNode = Process._getChildNode(node, "a:Entities");
                var entityNodes = Process._getChildNodes(entitiesNode, "a:Entity");

                value = [];
                if (entityNodes != null && entityNodes.length > 0) {
                    for (var i = 0; i < entityNodes.length; i++) {
                        value.push(Process._getEntityData(entityNodes[i]));
                    }
                }
            }
            else if (valueType == "aliasedvalue") {
                // Gets the actual data type of the aliased value
                // Key for these is "alias.fieldname"
                var aliasedValue = Process._getValue(Process._getChildNode(node, "a:Value"));
                if (aliasedValue != null) {
                    value = aliasedValue.value;
                    type = aliasedValue.type;
                }
            }
            else {
                // Standard fields like string, int, date, money, optionset, float, bool, decimal
                // Output will be string, even for number fields etc
                var stringValue = Process._getNodeTextValue(node);

                if (stringValue != null) {
                    switch (valueType) {
                        case "datetime":
                            value = new Date(stringValue);
                            break;
                        case "int":
                        case "money":
                        case "optionsetvalue":
                        case "double": // float
                        case "decimal":
                            value = Number(stringValue);
                            break;
                        case "boolean":
                            value = stringValue.toLowerCase() === "true";
                            break;
                        default:
                            value = stringValue;
                    }
                }
            }
        }
    }

    return new Process.Attribute(value, type);
}

Process._getEntityData = function (entityNode) {
    var value = null;

    var entityAttrsNode = Process._getChildNode(entityNode, "a:Attributes");
    var entityIdNode = Process._getChildNode(entityNode, "a:Id");
    var entityLogicalNameNode = Process._getChildNode(entityNode, "a:LogicalName");
    var entityFormattedValuesNode = Process._getChildNode(entityNode, "a:FormattedValues");

    var entityLogicalName = Process._getNodeTextValue(entityLogicalNameNode);
    var entityId = Process._getNodeTextValue(entityIdNode);
    var entityAttrs = Process._getChildNodes(entityAttrsNode, "a:KeyValuePairOfstringanyType");

    value = new Process.Entity(entityLogicalName, entityId);

    // Attribute values accessed via entity.attributes["new_fieldname"]
    if (entityAttrs != null && entityAttrs.length > 0) {
        for (var i = 0; i < entityAttrs.length; i++) {

            var attrNameNode = Process._getChildNode(entityAttrs[i], "b:key")
            var attrValueNode = Process._getChildNode(entityAttrs[i], "b:value");

            var attributeName = Process._getNodeTextValue(attrNameNode);
            var attributeValue = Process._getValue(attrValueNode);

            value.attributes[attributeName] = attributeValue;
        }
    }

    // Formatted values accessed via entity.formattedValues["new_fieldname"]
    for (var j = 0; j < entityFormattedValuesNode.childNodes.length; j++) {
        var foNode = entityFormattedValuesNode.childNodes[j];

        var fNameNode = Process._getChildNode(foNode, "b:key")
        var fValueNode = Process._getChildNode(foNode, "b:value");

        var fName = Process._getNodeTextValue(fNameNode);
        var fValue = Process._getNodeTextValue(fValueNode);

        value.formattedValues[fName] = fValue;
    }

    return value;
}

Process._getXmlValue = function (key, dataType, value) {
    var xml = "";
    var xmlValue = "";

    var extraNamespace = "";

    // Check the param type to determine how the value is formed
    switch (dataType) {
        case Process.Type.String:
            xmlValue = Process._htmlEncode(value) || ""; // Allows fetchXml strings etc
            break;
        case Process.Type.DateTime:
            xmlValue = value.toISOString() || "";
            break;
        case Process.Type.EntityReference:
            xmlValue = "<a:Id>" + (value.id || "") + "</a:Id>" +
                "<a:LogicalName>" + (value.entityType || "") + "</a:LogicalName>" +
                "<a:Name i:nil='true' />";
            break;
        case Process.Type.OptionSet:
        case Process.Type.Money:
            xmlValue = "<a:Value>" + (value || 0) + "</a:Value>";
            break;
        case Process.Type.Entity:
            xmlValue = Process._getXmlEntityData(value);
            break;
        case Process.Type.EntityCollection:
            if (value != null && value.length > 0) {
                var entityCollection = "";
                for (var i = 0; i < value.length; i++) {
                    var entityData = Process._getXmlEntityData(value[i]);
                    if (entityData !== null) {
                        entityCollection += "<a:Entity>" + entityData + "</a:Entity>";
                    }
                }
                if (entityCollection !== null && entityCollection !== "") {
                    xmlValue = "<a:Entities>" + entityCollection + "</a:Entities>" +
                        "<a:EntityName i:nil='true' />" +
                        "<a:MinActiveRowVersion i:nil='true' />" +
                        "<a:MoreRecords>false</a:MoreRecords>" +
                        "<a:PagingCookie i:nil='true' />" +
                        "<a:TotalRecordCount>0</a:TotalRecordCount>" +
                        "<a:TotalRecordCountLimitExceeded>false</a:TotalRecordCountLimitExceeded>";
                }
            }
            break;
        case Process.Type.Guid:
            // I don't think guid fields can even be null?
            xmlValue = value || Process._emptyGuid;

            // This is a hacky fix to get guids working since they have a conflicting namespace :(
            extraNamespace = " xmlns:c='http://schemas.microsoft.com/2003/10/Serialization/'";
            break;
        default: // bool, int, double, decimal
            xmlValue = value || null;
            break;
    }

    xml = "<a:KeyValuePairOfstringanyType>" +
        "<b:key>" + key + "</b:key>" +
        "<b:value i:type='" + dataType + "'" + extraNamespace;

    // nulls crash if you have a non-self-closing tag
    if (xmlValue === null || xmlValue === "") {
        xml += " i:nil='true' />";
    }
    else {
        xml += ">" + xmlValue + "</b:value>";
    }

    xml += "</a:KeyValuePairOfstringanyType>";

    return xml;
}

Process._getXmlEntityData = function (entity) {
    var xml = null;

    if (entity != null) {
        var attrXml = "";

        for (field in entity.attributes) {
            var a = entity.attributes[field];
            var aXml = Process._getXmlValue(field, a.type, a.value);

            attrXml += aXml;
        }

        if (attrXml !== "") {
            xml = "<a:Attributes>" + attrXml + "</a:Attributes>";
        }
        else {
            xml = "<a:Attributes />";
        }

        xml += "<a:EntityState i:nil='true' />" +
            "<a:FormattedValues />" +
            "<a:Id>" + entity.id + "</a:Id>" +
            "<a:KeyAttributes />" +
            "<a:LogicalName>" + entity.logicalName + "</a:LogicalName>" +
            "<a:RelatedEntities />" +
            "<a:RowVersion i:nil='true' />";
    }

    return xml;
}

Process._htmlEncode = function (s) {
    if (typeof s !== "string") { return s; }

    return s.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
}

Process.Entity = function (logicalName, id, attributes) {
    this.logicalName = logicalName || "";
    this.attributes = attributes || {};
    this.formattedValues = {};
    this.id = id || Process._emptyGuid;
}

// Gets the value of the attribute without having to check null
Process.Entity.prototype.get = function (key) {
    var a = this.attributes[key];
    if (a != null) {
        return a.value;
    }

    return null;
}

Process.EntityReference = function (entityType, id, name) {
    this.id = id || Process._emptyGuid;
    this.name = name || "";
    this.entityType = entityType || "";
}

Process.Attribute = function (value, type) {
    this.value = value || null;
    this.type = type || "";
}

/////////////////////////////Ahmed A.Ghany Library //////////////////////////////////////////////////////////////////////////////
function FilterLookUpReplaceParamters(controlName, entity, paramters, curformFields) {

    var defaultViewId = Xrm.Page.getControl(controlName).getDefaultView();
    var viewEntity = GetEntityByGuid('SavedQuery', 'SavedQueryId', defaultViewId).results[0];
    var formFields = null;
    var filterData = function () {
        //get defaul view id


        //Random Guid
        var viewId = '{1DFB2B35-B07C-44D1-868D-258DEEAB88E2}';
        var fetchXml = viewEntity.FetchXml;

        if (paramters != null && paramters != "") {
            var filterFields = paramters.split(',');
            var formFields = curformFields.split(',');
            for (var i = 0; i < filterFields.length; i++) {
                if (IsLookUp(formFields[i])) {
                    if (getLookUpValue(formFields[i]) != null && getLookUpValue(formFields[i]) != 'undefinded') {
                        fetchXml = fetchXml.replace(filterFields[i], getLookUpValue(formFields[i]));
                    }
                }
                else {
                    if (GetValueAttribute(formFields[i]) != null && GetValueAttribute(formFields[i]) != 'undefinded') {
                        fetchXml = fetchXml.replace(filterFields[i], GetValueAttribute(formFields[i]));
                    }
                }
            }
        }
        //alert(fetchXml);

        var LayoutXml = viewEntity.LayoutXml;
        //var Entity = viewEntity.entity;
        var QueryId = viewId;
        var viewDisplayName = "Filter View";
        Xrm.Page.getControl(controlName).addCustomView(viewId, entity, viewDisplayName, fetchXml, LayoutXml, true);
    }
    if (curformFields != null) {
        formFields = curformFields.split(',');
        for (var j = 0; j < formFields.length; j++) {
            BindOnChange(formFields[j], filterData);
        }
    }
    filterData();
}
function FilterLookUpReplaceParamtersMGA(controlName, entity, paramters, curformFields) {

    var defaultViewId = Xrm.Page.getControl(controlName).getDefaultView();
    var viewEntity = GetEntityByGuid('SavedQuery', 'SavedQueryId', defaultViewId).results[0];
    var formFields = null;
    var filterData = function () {
        //get defaul view id


        //Random Guid
        var viewId = '{1DFB2B35-B07C-44D1-868D-258DEEAB88E2}';
        var fetchXml = viewEntity.FetchXml;

        if (paramters != null && paramters != "") {
            var filterFields = paramters.split(',');
            var formFields = curformFields.split(',');
            for (var i = 0; i < filterFields.length; i++) {
                if (IsLookUp(formFields[i])) {
                    if (getLookUpValue(formFields[i]) != null && getLookUpValue(formFields[i]) != 'undefinded') {
                        fetchXml = fetchXml.replace(filterFields[i], getLookUpValue(formFields[i]));
                    }
                }
                else {
                    if (GetValueAttribute(formFields[i]) != null && GetValueAttribute(formFields[i]) != 'undefinded') {
                        fetchXml = fetchXml.replace(filterFields[i], GetValueAttribute(formFields[i]));
                    }
                }
            }
        }
        //alert(fetchXml);

        var LayoutXml = viewEntity.LayoutXml;
        //var Entity = viewEntity.entity;
        var QueryId = viewId;
        var viewDisplayName = "Filter View";
        Xrm.Page.getControl(controlName).addCustomView(viewId, entity, viewDisplayName, fetchXml, LayoutXml, true);
    }
    if (curformFields != null) {
        formFields = curformFields.split(',');
        for (var j = 0; j < formFields.length; j++) {
            //  BindOnChange(formFields[j], filterData);
        }
    }
    filterData();
}

function shortcut(shortcut, callback, opt) {
    //Provide a set of default options
    var default_options = {
        'type': 'keydown',
        'propagate': false,
        'target': document
    }
    if (!opt) opt = default_options;
    else {
        for (var dfo in default_options) {
            if (typeof opt[dfo] == 'undefined') opt[dfo] = default_options[dfo];
        }
    }
    var ele = opt.target
    if (typeof opt.target == 'string') ele = document.getElementById(opt.target);
    var ths = this;
    //The function to be called at keypress
    var func = function (e) {
        e = e || window.event;
        //Find Which key is pressed
        if (e.keyCode) code = e.keyCode;
        else if (e.which) code = e.which;
        var character = String.fromCharCode(code).toLowerCase();
        var keys = shortcut.toLowerCase().split("+");
        //Key Pressed - counts the number of valid keypresses - if it is same as the number of keys, the shortcut function is invoked
        var kp = 0;
        //Work around for stupid Shift key bug created by using lowercase - as a result the shift+num combination was broken
        var shift_nums = {
            "`": "~",
            "1": "!",
            "2": "@",
            "3": "#",
            "4": "$",
            "5": "%",
            "6": "^",
            "7": "&",
            "8": "*",
            "9": "(",
            "0": ")",
            "-": "_",
            "=": "+",
            ";": ":",
            "'": "\"",
            ",": "<",
            ".": ">",
            "/": "?",
            "\\": "|"
        }
        //Special Keys - and their codes
        var special_keys = {
            'esc': 27,
            'escape': 27,
            'tab': 9,
            'space': 32,
            'return': 13,
            'enter': 13,
            'backspace': 8,
            'scrolllock': 145,
            'scroll_lock': 145,
            'scroll': 145,
            'capslock': 20,
            'caps_lock': 20,
            'caps': 20,
            'numlock': 144,
            'num_lock': 144,
            'num': 144,
            'pause': 19,
            'break': 19,
            'insert': 45,
            'home': 36,
            'delete': 46,
            'end': 35,
            'pageup': 33,
            'page_up': 33,
            'pu': 33,
            'pagedown': 34,
            'page_down': 34,
            'pd': 34,
            'left': 37,
            'up': 38,
            'right': 39,
            'down': 40,
            'f1': 112,
            'f2': 113,
            'f3': 114,
            'f4': 115,
            'f5': 116,
            'f6': 117,
            'f7': 118,
            'f8': 119,
            'f9': 120,
            'f10': 121,
            'f11': 122,
            'f12': 123
        }
        for (var i = 0; k = keys[i], i < keys.length; i++) {
            //Modifiers
            if (k == 'ctrl' || k == 'control') {
                if (e.ctrlKey) kp++;
            } else if (k == 'shift') {
                if (e.shiftKey) kp++;
            } else if (k == 'alt') {
                if (e.altKey) kp++;
            } else if (k.length > 1) { //If it is a special key
                if (special_keys[k] == code) kp++;
            } else { //The special keys did not match
                if (character == k) kp++;
                else {
                    if (shift_nums[character] && e.shiftKey) { //Stupid Shift key bug created by using lowercase
                        character = shift_nums[character];
                        if (character == k) kp++;
                    }
                }
            }
        }
        if (kp == keys.length) {
            callback(e);
            if (!opt['propagate']) { //Stop the event
                //e.cancelBubble is supported by IE - this will kill the bubbling process.
                e.cancelBubble = true;
                e.returnValue = false;
                //e.stopPropagation works only in Firefox.
                if (e.stopPropagation) {
                    e.stopPropagation();
                    e.preventDefault();
                }
                return false;
            }
        }
    }
    //Attach the function with the event
    if (ele.addEventListener) ele.addEventListener(opt['type'], func, false);
    else if (ele.attachEvent) ele.attachEvent('on' + opt['type'], func);
    else ele['on' + opt['type']] = func;
}

shortcut('Ctrl+Q', CollapseAll);
shortcut('Ctrl+W', ExpandAll);
var parentOptions = {

    'target': document.parentDocument
};
shortcut('Ctrl+Q', CollapseAll, parentOptions);
shortcut('Ctrl+W', ExpandAll, parentOptions);

String.prototype.CustomReplace = function (value, valuetoReplace) {
    var x = this.replace(value, valuetoReplace);
    if (x.indexOf(value) >= 0)
        x = x.CustomReplace(value, valuetoReplace);
    return x;
}


function SetLookupValue(fieldName, id, name, entityType) {
    if (fieldName != null) {
        if (id == null || name == null) {
            SetValueAttribute(fieldName, null);
            return;
        }
        var lookupValue = new Array();
        lookupValue[0] = new Object();
        lookupValue[0].id = id;
        lookupValue[0].name = name;
        lookupValue[0].entityType = entityType;
        Xrm.Page.getAttribute(fieldName).setValue(lookupValue);
    }


}

function GetFormID() {
    if (Xrm.Page.data != null) {
        var GUIDvalue = Xrm.Page.data.entity.getId();
        return GUIDvalue;
    }
    else {
        return null;
    }
}
function RefreshGrid(gridName) {
    var targetgird = Xrm.Page.ui.controls.get(gridName);
    targetgird.Refresh();
}
function GetEntityByID(entityName, fields, id) {

    var arrvalue = new Array();
    var url = 'https://' + window.location.hostname + ":8000/GetEntityDataById.ashx?Id=" + id + "&Entity=" + entityName + "&Columns=" + fields;
    url += '&time=' + new Date().getTime();
    str = callAjax(url);
    eval(str);
    return arrvalue;
}
function GetEntityByGuid(entityName, searchByColumn, searchByValue) {


    //var serverUrl = Xrm.Page.context.getServerUrl();
    var url = getServerurl() + "/xrmservices/2011/OrganizationData.svc/" + entityName + "Set?$filter=" + searchByColumn + " eq guid'" + searchByValue + "'";
    str = CallOData(url);
    var data = eval('(' + str + ')');
    return data.d;
}
function SelectEntityByGuid(entityName, columns, searchByColumn, searchByValue) {


    //var serverUrl = Xrm.Page.context.getServerUrl();
    var url = getServerurl() + "/xrmservices/2011/OrganizationData.svc/" + entityName + "Set?$select=" + columns + "&$filter=" + searchByColumn + " eq guid'" + searchByValue + "'";
    str = CallOData(url);
    var data = eval('(' + str + ')');
    return data.d;
}
function getServerurl() {
    var serverUrl = "https://" + window.location.hostname + (window.location.port ? ':' + window.location.port : '') + "/" + Xrm.Page.context.getOrgUniqueName();
    return serverUrl;
}
function GetEntityByRefrenceID(entityName, searchByColumn, searchByValue) {


    //var serverUrl = Xrm.Page.context.getServerUrl();
    var url = getServerurl() + "/xrmservices/2011/OrganizationData.svc/" + entityName + "Set?$filter=" + searchByColumn + "/Id  eq guid'" + searchByValue + "'";
    str = CallOData(url);
    var data = eval('(' + str + ')');
    return data.d;
}
function SelectEntityByRefrenceID(entityName, columns, searchByColumn, searchByValue) {


    //var serverUrl = Xrm.Page.context.getServerUrl();
    var url = getServerurl() + "/xrmservices/2011/OrganizationData.svc/" + entityName + "Set?$select=" + columns + "&$filter=" + searchByColumn + "/Id  eq guid'" + searchByValue + "'";
    str = CallOData(url);
    var data = eval('(' + str + ')');
    return data.d;
}
function GetEntityByString(entityName, searchByColumn, searchByValue) {


    //var serverUrl = Xrm.Page.context.getServerUrl();
    var url = getServerurl() + "/xrmservices/2011/OrganizationData.svc/" + entityName + "Set?$filter=" + searchByColumn + " eq '" + searchByValue + "'";
    str = CallOData(url);
    var data = eval('(' + str + ')');
    return data.d;
}
//
function SelectEntityByString(entityName, columns, searchByColumn, searchByValue) {


    //var serverUrl = Xrm.Page.context.getServerUrl();
    var url = getServerurl() + "/xrmservices/2011/OrganizationData.svc/" + entityName + "Set?$select=" + columns + "&$filter=" + searchByColumn + " eq '" + searchByValue + "'";
    str = CallOData(url);
    var data = eval('(' + str + ')');
    return data.d;
}
function GetEntityByDate(entityName, searchByColumn, searchByValue) {


    //var serverUrl = Xrm.Page.context.getServerUrl();
    var url = getServerurl() + "/xrmservices/2011/OrganizationData.svc/" + entityName + "Set?$filter=" + searchByColumn + " eq datetime'" + searchByValue + "'";
    str = CallOData(url);
    var data = eval('(' + str + ')');
    return data.d;
}

function GetEntityByNumber(entityName, searchByColumn, searchByValue) {


    //var serverUrl = Xrm.Page.context.getServerUrl();
    var url = getServerurl() + "/xrmservices/2011/OrganizationData.svc/" + entityName + "Set?$filter=" + searchByColumn + " eq " + searchByValue + "";
    str = CallOData(url);
    var data = eval('(' + str + ')');
    return data.d;
}

function callAjax(url) {
    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    }
    else {// code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    xmlhttp.open("GET", url, false);
    xmlhttp.setRequestHeader("X-Requested-Width", "XMLHttpRequest");
    xmlhttp.setRequestHeader("Accept", "application/json, text/javascript, */*");
    xmlhttp.send(null);

    return xmlhttp.responseText;

}

function CallOData(url) {
    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    }
    else {// code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    xmlhttp.open("GET", url, false);
    xmlhttp.setRequestHeader("X-Requested-Width", "XMLHttpRequest");
    xmlhttp.setRequestHeader("Accept", "application/json, text/javascript, */*");
    xmlhttp.send(null);
    return xmlhttp.responseText;

}
function GetOdataResults(url) {
    CallOData(url);
    str = CallOData(url);
    var data = eval('(' + str + ')');
    return data.d;
}

function SetEnabledState(controlName, value) {
    var AddressType = Xrm.Page.ui.controls.get(controlName);
    AddressType.setDisabled(value);
}


function setEnabled(controlName) {
    var AddressType = Xrm.Page.ui.controls.get(controlName);
    AddressType.setDisabled(false);
}
function setDisabled(controlName) {
    var AddressType = Xrm.Page.ui.controls.get(controlName);
    AddressType.setDisabled(true);
}
function SetDisabledWithSaving(controlName) {
    setDisabled(controlName)
    Xrm.Page.getAttribute(controlName).setSubmitMode("always");
}

function getLookUpValue(controlName) {
    var ExistingCase = Xrm.Page.data.entity.attributes.get(controlName);
    if (ExistingCase.getValue() == null) { return null; };
    var ExistingCaseGUID = ExistingCase.getValue()[0].id;
    return ExistingCaseGUID;
}
function getLookUpType(controlName) {
    var ExistingCase = Xrm.Page.data.entity.attributes.get(controlName);
    if (ExistingCase.getValue() == null) { return null; };
    var ExistingCaseGUID = ExistingCase.getValue()[0].entityType;
    return ExistingCaseGUID;
}

function getLookUpDisplay(controlName) {

    var ExistingCase = Xrm.Page.data.entity.attributes.get(controlName);
    if (ExistingCase.getValue() == null) { return null; };
    var ExistingCaseGUID = ExistingCase.getValue()[0].name;
    return ExistingCaseGUID;
}
function onCustomerChange() {

    var id = Xrm.Page.data.entity.attributes.get("new_customeraccount").getValue()[0].id;
    var account = GetEntityByID('account', 'accountid,name,new_totalincome,new_totalcommitment,new_accountstype', id);
    crmForm.all.new_customeraverageincome.DataValue = checkNumber(account["new_totalincome"]);
    crmForm.all.new_customeravergaecommitment.DataValue = checkNumber(account["new_totalcommitment"]);

    crmForm.all.new_customeraccounttype.DataValue = checkNumber(account["new_accountstype"]);
    CustomerfinanceCalCulator();
}

function DisableSection(sectionName) {
    //disable all the controls that has section as parent
    Xrm.Page.ui.controls.forEach(function (control, index) {
        if (control.getParent() != null) {
            if (control.getParent().getName() == sectionName) {
                if (control.getControlType() != 'subgrid') {
                    //                if (control.getControlType() == 'lookup') {
                    //                    DisableLookup(control.getName());
                    //                }
                    control.setDisabled(true);
                }
            }
        }
    });
}

function EnableSection(sectionName) {
    //disable all the controls that has section as parent
    Xrm.Page.ui.controls.forEach(function (control, index) {
        if (control.getParent() != null && control.getParent().getName() == sectionName) {
            if (control.getControlType() != 'subgrid') {
                control.setDisabled(false);
            }
        }
    });
}

function SetSectionMandatory(sectionName) {
    Xrm.Page.ui.controls.forEach(function (control, index) {
        if (control.getParent().getName() == sectionName) {
            if (control.getName() != null) {
                if (control.getName().indexOf('note') <= 0) {
                    SetMandatory(control.getName());
                }
            }


        }
    });
}

function RemoveSectionMandatory(sectionName) {
    Xrm.Page.ui.controls.forEach(function (control, index) {
        if (control.getParent() != null) {
            if (control.getParent().getName() == sectionName) {
                RemoveMandatory(control.getName());
            }
        }
    });
}
function onSponsorChange() {

    var id = Xrm.Page.data.entity.attributes.get("new_sponsor").getValue()[0].id;
    var account = GetEntityByID('account', 'accountid,name,new_totalincome,new_totalcommitment,new_accountstype', id);
    crmForm.all.new_sponsoraverageincome.DataValue = checkNumber(account["new_totalincome"]);
    crmForm.all.new_sponsoraveragecommitment.DataValue = checkNumber(account["new_totalcommitment"]);
    crmForm.all.new_sponsoraccounttype.DataValue = checkNumber(account["new_accountstype"]);
    initalizeForm();
    CustomerfinanceCalCulator();
}
function checkNumber(num) {
    if (num == null)
        return null;
    else {
        return Number(num);
    }
}

function HideShowSection(tabName, sectionName, visible) {

    Xrm.Page.ui.tabs.get(tabName).sections.get(sectionName).setVisible(visible);
}

function SetMandatory(fieldName) {
    if (Xrm.Page.getAttribute(fieldName) != null && Xrm.Page.getAttribute(fieldName).setRequiredLevel != null) {
        Xrm.Page.getAttribute(fieldName).setRequiredLevel("required");
    }
}
function setToggleMandatory(fieldName, valueForRequired, mandatoryFields) {
    var fncToggle = function () {
        var fields = mandatoryFields.split(',');
        var fieldValue = GetValueAttribute(fieldName);
        for (i = 0; i < fields.length; i++) {
            if (fieldValue == valueForRequired)
                SetMandatory(fields[i]);
            else
                RemoveMandatory(fields[i]);
        }
    };
    BindOnChange(fieldName, fncToggle);
    fncToggle();
}
function SetDisableWhen(fieldName, valueForRequired, disableFields) {
    var fields = disableFields.split(',');
    var fieldValue = GetValueAttribute(fieldName);
    if (fieldValue == valueForRequired)
        SetEnabledState(fieldName, true);
    else
        SetEnabledState(fieldName, false);
    for (i = 0; i < fields.length; i++) {
        if (fieldValue == valueForRequired)
            SetEnabledState(fields[i], true);
        else
            SetEnabledState(fields[i], false);
    }
}
function SetEnableWhen(fieldName, valueForRequired, disableFields) {
    var fields = disableFields.split(',');
    var fieldValue = GetValueAttribute(fieldName);
    if (fieldValue == valueForRequired) {
        for (i = 0; i < fields.length; i++) {

            SetEnabledState(fields[i], false);
        }
    }
}
function setMandatory(fieldName, valueForRequired) {
    var fieldValue = GetValueAttribute(fieldName);
    if (valueForRequired == fieldValue)
        SetMandatory(fieldName);
    else
        RemoveMandatory(fieldName);
}
function RemoveMandatory(fieldName) {
    if (Xrm.Page.getAttribute(fieldName) != null && Xrm.Page.getAttribute(fieldName).setRequiredLevel != null) {
        Xrm.Page.getAttribute(fieldName).setRequiredLevel("none");
    }
}
function SetTabVisibility(tabName, value) {
    if (Xrm.Page.ui.tabs.get(tabName)) {
        Xrm.Page.ui.tabs.get(tabName).setVisible(value);
    }
    else {
        alert(tabName);
    }
}

function SetTabCollapse(tabName) {
    Xrm.Page.ui.tabs.get(tabName).setDisplayState('collapsed');
}
function SetTabExpand(tabName) {
    Xrm.Page.ui.tabs.get(tabName).setDisplayState('expanded');
}

function CollapseAll() {
    Xrm.Page.ui.tabs.forEach(
        function (control) {
            control.setDisplayState('collapsed');
        }
    );

}
function ExpandAll() {
    Xrm.Page.ui.tabs.forEach(
        function (control) {
            control.setDisplayState('expanded');
        }
    );
}


function SetSectionVisablity(sectionName, sectionIsVisible) {
    //Hide or Show Sections 
    var tabs = Xrm.Page.ui.tabs.get();
    for (var i in tabs) {
        var tab = tabs[i];
        tab.sections.forEach(function (section, index) {
            if (section.getName() == sectionName) {
                section.setVisible(sectionIsVisible);
            }
        });
    }
}
function GetSection(sectionName) {
    //Hide or Show Sections 
    var tabs = Xrm.Page.ui.tabs.get();
    for (var i in tabs) {
        var tab = tabs[i];
        tab.sections.forEach(function (section, index) {
            if (section.getName() == sectionName) {
                return section;
            }
        });
    }
}
function SetTabVisibilityWhen(tabName, boolVisibleValue, dependOnFieldName, values) {
    var fieldvalue = GetValueAttribute(dependOnFieldName);
    if (fieldvalue != null) {
        if (values.indexOf(fieldvalue) >= 0) {
            SetTabVisibility(tabName, boolVisibleValue);
        }
        else {
            SetTabVisibility(tabName, !boolVisibleValue);
        }
    }
    else {
        SetTabVisibility(tabName, !boolVisibleValue);
    }
}
function onPaymentMethod() {

    var value = Xrm.Page.data.entity.attributes.get("new_paymentmethod").getValue();
    if (value == null) return;
    var sections = new Array();
    sections['100000000'] = 'cash';
    sections['100000001'] = 'cheque';
    sections['100000002'] = 'transfer';
    sections['100000003'] = 'atm';
    for (key in sections) {
        if (key == value) {
            HideShowSection('GeneralTab', sections[key], true);
            //  SetSectionMandatory(sections[key]);

        }
        else {
            try {
                RemoveSectionMandatory(sections[key]);
            }
            catch (e) { }
            HideShowSection('GeneralTab', sections[key], false);

        }
    }

}
function GetAttribute(attributeName) {
    return Xrm.Page.getAttribute(attributeName);
}
function GetValueAttribute(attributeName) {
    if (Xrm.Page.getAttribute(attributeName))
        return Xrm.Page.getAttribute(attributeName).getValue();
}
function SetValueAttribute(attributeName, Value) {
    Xrm.Page.getAttribute(attributeName).setValue(Value);
}
function SetVisible(controlName, value) {
    Xrm.Page.getControl(controlName).setVisible(value)
}
function SetFocus(controlName) {
    Xrm.Page.getControl(controlName).setFocus(true);
}
function IsCreateForm() {
    if (Xrm.Page.ui.getFormType() == 1) {
        return true;
    }

    return false;
}
function IsEditForm() {
    if (Xrm.Page.ui.getFormType() == 2) {
        return true;
    }

    return false;
}
function bindTafkeet(cNumberName, cWordsName) {

    var controlNumber = document.getElementById(cNumberName);
    if (controlNumber == null) {
        alert(cNumberName + " is not Exist");
        return;
    }

    controlNumber.setAttribute('onchange', function () {
        var words = callAjax("http://" + window.location.hostname + ":8000/Tafkeet.ashx?Number=" + controlNumber.value);
        Xrm.Page.getAttribute(cWordsName).setValue(words);
    });

    if (GetValueAttribute(cNumberName) > 0 && GetValueAttribute(cWordsName) == null) {
        var words = callAjax("http://" + window.location.hostname + ":8000/Tafkeet.ashx?Number=" + controlNumber.value);
        Xrm.Page.getAttribute(cWordsName).setValue(words);
    }

}
function confirmYesNo(str) {
    execScript('n = msgbox("' + str + '","3")', "vbscript");
    return (n == 6);
}
function MessageBox(strMessage, title) {
    var strScript = 'n = msgbox("' + strMessage + '","3","' + title + '")';
    strScript = strScript.replace('nn', '"& chr(13) &"');
    execScript(strScript, "vbscript");
    if (n == 6) return 'yes';
    if (n == 7) return 'no';
    if (n == 2) return 'cancel';
    alert(n);
}

///******************************************************************************
function SaveAndCloseForm() {
    Xrm.Page.data.entity.save('saveandclose');
}
function SaveAndNew() {
    Xrm.Page.data.entity.save('saveandnew');
}
function openNewWindow(url) {
    var name = "newWindow";
    var width = 800;
    var height = 600;
    var newWindowFeatures = "status=1;scrollbars=1";

    // CRM function to open a new window
    return OpenNewStandardWidow(url, name, width, height, newWindowFeatures);

}

function OpenNewStandardWidow(url) {
    mywindow = window.open(url, "mywindow", "scrollbars=yes,resizable=yes,  width=800,height=600");

}
function bindLookUpToName(controlName, nameField) {

    var control = document.getElementById(controlName);
    var tempFunc =
        function () {
            var x = getLookUpDisplay(controlName);
            //alert(x);
            if (nameField == null) nameField = 'new_name';
            SetValueAttribute(nameField, x);
        };

    Xrm.Page.data.entity.attributes.get(controlName).addOnChange(tempFunc);
}
function BindOnChange(controlName, fnction) {

    Xrm.Page.data.entity.attributes.get(controlName).addOnChange(fnction);
}
function bindOnSave(fnction) {
    Xrm.Page.data.entity.addOnSave(fnction)
}
function bindSelectToName(controlName, nameField) {

    var control = document.getElementById(controlName);
    control.onchange =
        function () {
            var x = control[control.selectedIndex].text;
            if (nameField == null) nameField = 'new_name';
            SetValueAttribute(nameField, x);
        };

}

var LoadedArray = new Array();

function CalculateField(fieldName, equation) {
    var MainEquation = '';
    equation = equation.toLowerCase();
    MainEquation = equation;
    var tempEquation = equation.CustomReplace("\*", ',').CustomReplace('/', ',').CustomReplace('+', ',').CustomReplace('-', ',');
    tempEquation = tempEquation.CustomReplace('.', '').CustomReplace('0', '').CustomReplace('1', '').CustomReplace('2', '');
    tempEquation = tempEquation.CustomReplace('3', '').CustomReplace('4', '').CustomReplace('5', '').CustomReplace('6', '');
    tempEquation = tempEquation.CustomReplace('7', '').CustomReplace('8', '').CustomReplace('9', '');
    tempEquation = tempEquation.CustomReplace('(', '').CustomReplace(')', '');
    var controls = tempEquation.split(',');
    for (var i = 0; i < controls.length; i++) {
        if (controls[i] == '' || controls[i] == null) continue;
        var attribute = GetAttribute(controls[i]);
        if (LoadedArray[fieldName] == undefined) {
            var tempFunc = function () {
                CalculateField(fieldName, equation);
                FireOnChange(fieldName);
            };
            //attribute.addOnChange(tempFunc);
            if (controls[i] != '') {

                BindOnChange(controls[i], tempFunc);
            }
            // Xrm.Page.data.entity.attributes.get(controls[i]).addOnChange(tempFunc);

        }
        if (controls[i] != "") {
            var value = GetValueAttribute(controls[i]);
            if (value == undefined) value = 0;
            MainEquation = MainEquation.replace(controls[i], value);
        }

    }
    SetValueAttribute(fieldName, eval(MainEquation.replace('--', '+')));

    // ChangeFieldBackGround(fieldName, 'yellow');
    //SetEnabledState(fieldName, true);
    if (LoadedArray[fieldName] == undefined) {
        var tempDisableFunc = function () {
            SetEnabledState(fieldName, false);
        }
        Xrm.Page.data.entity.addOnSave(tempDisableFunc);
    }
    LoadedArray[fieldName] = 1;

}

function disableFormFields(onOff) {

    Xrm.Page.ui.controls.forEach(function (control, index) {
        if (doesControlHaveAttribute(control)) {
            if (control.getControlType() == 'lookup') {
                if (onOff == true) {
                    // DisableLookup(control.getName());
                }
            }
            control.setDisabled(onOff);
        }
    });
}
function removeMandatoryFormFields() {

    Xrm.Page.ui.controls.forEach(function (control, index) {
        if (doesControlHaveAttribute(control)) {
            setMandatory(control.getName(), false);

        }
    });
}

function doesControlHaveAttribute(control) {
    var controlType = control.getControlType();
    return controlType != "iframe" && controlType != "webresource" && controlType != "subgrid";
}

function disableSubgrid(subgridName) {
    var gridSpan = document.getElementById(subgridName + "_span");
    if (gridSpan.readyState != 'complete') {
        setTimeout("disableSubgrid('" + subgridName + "')", 1000);
    }

}
function calculateDetails(detailsEntityName, searchColumnName, fieldsComma) {

    var data = SelectEntityByRefrenceID(detailsEntityName, fieldsComma, searchColumnName, GetFormID());
    var fields = fieldsComma.split(',');
    var values = new Array();
    if (data.results.length > 0) {
        for (var i = 0; i < data.results.length; i++) {
            for (var j = 0; j < fields.length; j++) {
                if (values[fields[j]] == null) values[fields[j]] = 0;
                var attribute = eval('data.results[i].' + fields[j]);
                if (attribute != null && attribute.Value == null) {

                    values[fields[j]] = values[fields[j]] + Number(attribute);
                }
                if (attribute != null && attribute.Value != null) {

                    values[fields[j]] = values[fields[j]] + Number(attribute.Value);
                }
            }
        }
    }
    else {
        for (var j = 0; j < fields.length; j++) {
            values[fields[j]] = 0;
        }

    }
    return values;
}
function BindSubgridRefresh(gridName, gridEntity, filterByColumn, calculatedFields, totalFields) {

    /* CRM 2011
    //var grid = document.getElementById(gridName);
    var grid = Xrm.Page.getControl(gridName)._control.get_innerControl();
    //alert("ahmed");
    if (grid == null) {
    setTimeout("BindSubgridRefresh('" + gridName + "','" + gridEntity + "', '" + filterByColumn + "', '" + calculatedFields + "', '" + totalFields + "');", 1000);
    return;
    }

    // alert("ahmed");
    grid._events.addHandler('OnRefresh', function () { SetCalculatedFields(gridEntity, filterByColumn, calculatedFields, totalFields); });
    // grid.attachEvent("OnRefresh", function () { SetCalculatedFields(gridEntity, filterByColumn, calculatedFields, totalFields); });
    */


    //    function (){ SetCalculatedFields(gridEntity, filterByColumn, calculatedFields, totalFields); };

    SetCalculatedFields(gridEntity, filterByColumn, calculatedFields, totalFields);
    bindOnSave(function () { SetCalculatedFields(gridEntity, filterByColumn, calculatedFields, totalFields); })


}
function SetCalculatedFields(gridEntity, filterByColumn, calculatedFields, totalFields) {
    //alert("calc");
    if (IsEditForm()) {

        var arrValues = calculateDetails(gridEntity, filterByColumn, calculatedFields);
        var columns = totalFields.split(',');
        var fieldsValue = calculatedFields.split(',');
        for (var i = 0; i < columns.length; i++) {
            SetValueAttribute(columns[i], arrValues[fieldsValue[i]]);
            FireOnChange(columns[i]);
        }
    }

}
function FireOnChange(fielName) {
    Xrm.Page.getAttribute(fielName).fireOnChange();
}

function AddLabels(srcEntityNameDBCase, srcColumnsDBCase, SrcSearchFieldDBCase, frmSearchField, afterControlName, noOfColumns) {

    var GetColumnValue = function (entityColumn) {
        if (entityColumn == null)
            return "";

        if (entityColumn.Value != undefined) {
            return entityColumn.Value;
        }
        if (entityColumn.__metadata != null && entityColumn.__metadata.type == "Microsoft.Crm.Sdk.Data.Services.EntityReference") {
            if (entityColumn.Name != null) {
                return entityColumn.Name;
            }
            else return "";
        }

        if (entityColumn != null)
            return entityColumn;
    };

    var control = document.getElementById(afterControlName);

    var searchValue = getLookUpValue(frmSearchField);
    var parentTable = null;
    var InsertBeforeControl = null;
    if (control.tagName.toLowerCase() == "input") {
        parentTable = control.parentNode.parentNode.parentNode;
    }
    else {
        parentTable = control.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;
    }
    var Columns = srcColumnsDBCase.split(',');
    //var DisplayColumns = Labels.split(',');
    var entity = new Array();
    if (searchValue != null) {
        entity = GetEntityByGuid(srcEntityNameDBCase, SrcSearchFieldDBCase, getLookUpValue(frmSearchField)).results[0];
    }
    var tr = null;
    for (var i = 0; i < Columns.length; i++) {

        if (i % noOfColumns == 0) {
            tr = document.createElement('tr');
        }
        var td = document.createElement('td');
        td.setAttribute('classs', 'ms-crm-FieldLabel-LeftAlign ms-crm-Field-Normal');

        var label = document.createElement('label');
        // label.innerText = DisplayColumns[i];
        RetrieveAttributeLabel(srcEntityNameDBCase, Columns[i], label);
        td.appendChild(label);
        tr.appendChild(td);



        td = document.createElement('td');
        td.setAttribute('classs', 'ms-crm-FieldLabel-LeftAlign ms-crm-Field-Normal');
        td.setAttribute('nowrap', 'nowrap');
        label = document.createElement('span');

        if (searchValue != null) {
            var attribute = eval('entity.' + Columns[i]);
            var isOptionSet = false;

            if (attribute.__metadata != null && attribute.__metadata.type == "Microsoft.Crm.Sdk.Data.Services.OptionSetValue") {
                isOptionSet = true;
            }
            if (Columns[i].indexOf('status') < 0 && isOptionSet == false) {
                label.innerHTML = "<nobr class='ms-crm-Lookup-Item'>" + GetColumnValue(eval('entity.' + Columns[i])) + "</nobr>";
                label.setAttribute('title', GetColumnValue(eval('entity.' + Columns[i])));
                label.setAttribute('class', "ms-crm-Lookup-Item");
            }
            else {
                label.innerHTML = "<nobr class='ms-crm-Lookup-Item'>" + GetColumnValue(eval('entity.' + Columns[i])) + "</nobr>";
                label.setAttribute('title', GetColumnValue(eval('entity.' + Columns[i])));
                label.setAttribute('class', "ms-crm-Lookup-Item");
                RetrieveOptionsetLabel(srcEntityNameDBCase, Columns[i], attribute.Value, label);
            }
        }
        td.appendChild(label);
        tr.appendChild(td);

        if ((i + 1) % noOfColumns == 0) {
            parentTable.appendChild(tr);

        }
    }
    if (tr.parentNode == null) {
        parentTable(tr);
    }


}

//document.body.
function AddLabelsHorizontal(srcEntityNameDBCase, srcColumnsDBCase, SrcSearchFieldDBCase, frmSearchField, afterControlName, noOfColumns) {

    var GetColumnValue = function (entityColumn) {
        if (entityColumn == null)
            return "";

        if (entityColumn.Value != undefined) {
            return entityColumn.Value;
        }
        if (entityColumn.__metadata != null && entityColumn.__metadata.type == "Microsoft.Crm.Sdk.Data.Services.EntityReference") {
            if (entityColumn.Name != null) {
                return entityColumn.Name;
            }
            else return "";
        }

        if (entityColumn != null)
            return entityColumn;
    };

    var control = document.getElementById(afterControlName);

    var searchValue = getLookUpValue(frmSearchField);
    var parentTable = null;
    var InsertBeforeControl = null;
    var currentRow;
    if (control.tagName.toLowerCase() == "input") {
        parentTable = control.parentNode.parentNode.parentNode;
    }
    else {
        currentRow = control.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;
        parentTable = control.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;
    }
    currentRow.removeChild(currentRow.lastChild);
    //currentRow.removeChild(currentRow.lastChild);
    var Columns = srcColumnsDBCase.split(',');
    //var DisplayColumns = Labels.split(',');
    var entity = new Array();
    if (searchValue != null) {
        entity = GetEntityByGuid(srcEntityNameDBCase, SrcSearchFieldDBCase, getLookUpValue(frmSearchField)).results[0];
    }
    var tr = null;
    for (var i = 0; i < Columns.length; i++) {

        if (i % noOfColumns == 0) {
            tr = document.createElement('tr');
        }
        if (i > 0) {
            currentRow = tr;
        }
        var td = document.createElement('td');
        td.setAttribute('classs', 'ms-crm-FieldLabel-LeftAlign ms-crm-Field-Normal');

        var label = document.createElement('label');
        // label.innerText = DisplayColumns[i];
        RetrieveAttributeLabel(srcEntityNameDBCase, Columns[i], label);
        td.appendChild(label);
        currentRow.appendChild(td);



        td = document.createElement('td');
        td.setAttribute('classs', 'ms-crm-FieldLabel-LeftAlign ms-crm-Field-Normal');
        td.setAttribute('nowrap', 'nowrap');
        label = document.createElement('span');

        if (searchValue != null) {
            var attribute = eval('entity.' + Columns[i]);
            var isOptionSet = false;

            if (attribute.__metadata != null && attribute.__metadata.type == "Microsoft.Crm.Sdk.Data.Services.OptionSetValue") {
                isOptionSet = true;
            }
            if (Columns[i].indexOf('status') < 0 && isOptionSet == false) {
                label.innerHTML = "<nobr class='ms-crm-Lookup-Item'>" + GetColumnValue(eval('entity.' + Columns[i])) + "</nobr>";
                label.setAttribute('title', GetColumnValue(eval('entity.' + Columns[i])));
                label.setAttribute('class', "ms-crm-Lookup-Item");
            }
            else {
                label.innerHTML = "<nobr class='ms-crm-Lookup-Item'>" + GetColumnValue(eval('entity.' + Columns[i])) + "</nobr>";
                label.setAttribute('title', GetColumnValue(eval('entity.' + Columns[i])));
                label.setAttribute('class', "ms-crm-Lookup-Item");
                RetrieveOptionsetLabel(srcEntityNameDBCase, Columns[i], attribute.Value, label);
            }
        }
        td.appendChild(label);
        currentRow.appendChild(td);
        //  tr.appendChild(td);

        if ((i + 1) % noOfColumns == 0 && i > 0) {
            parentTable.appendChild(tr);

        }
    }
    if (tr.parentNode == null) {
        //parentTable(tr);
    }


}
function RetrieveAttributeLabel(entityName, attributeName, control) {
    // Entity schema name
    var entityLogicalName = entityName;
    // option set schema name
    var RetrieveAttributeName = attributeName;
    // Target Field schema name to which optionset text needs to be assigned
    var AssignAttributeName = control;

    // Option set value for which label needs to be retrieved

    var onSucessFn = function (logicalName, entityMetadata, RetrieveAttributeName, AssignAttributeName) {
        ///<summary>
        /// Retrieves attributes for the entity 
        ///</summary>

        var success = false;
        for (var i = 0; i < entityMetadata.Attributes.length; i++) {
            var AttributeMetadata = entityMetadata.Attributes[i];
            if (success) break;
            if (AttributeMetadata.SchemaName.toLowerCase() == RetrieveAttributeName.toLowerCase()) {
                control.innerText = AttributeMetadata.DisplayName.UserLocalizedLabel.Label;
                success = true;
                break;

            }

        }


    };

    // Calling Metadata service to get Optionset Label
    SDK.MetaData.RetrieveEntityAsync(SDK.MetaData.EntityFilters.Attributes, entityLogicalName, null, false, function (entityMetadata) { onSucessFn(entityLogicalName, entityMetadata, RetrieveAttributeName, AssignAttributeName); }, errorDisplay);

}

function RetrieveOptionsetLabel(entityName, attributeName, value, control) {
    // Entity schema name
    var entityLogicalName = entityName;
    // option set schema name
    var RetrieveAttributeName = attributeName;
    // Target Field schema name to which optionset text needs to be assigned
    var AssignAttributeName = control;

    // Option set value for which label needs to be retrieved
    var stateValue = value;


    // Calling Metadata service to get Optionset Label
    SDK.MetaData.RetrieveEntityAsync(SDK.MetaData.EntityFilters.Attributes, entityLogicalName, null, false, function (entityMetadata) { successRetrieveEntity(entityLogicalName, entityMetadata, RetrieveAttributeName, stateValue, AssignAttributeName); }, errorDisplay);

}

// Called upon successful metadata retrieval of the entity
function successRetrieveEntity(logicalName, entityMetadata, RetrieveAttributeName, OptionValue, AssignAttributeName) {
    ///<summary>
    /// Retrieves attributes for the entity 
    ///</summary>

    var success = false;
    for (var i = 0; i < entityMetadata.Attributes.length; i++) {
        var AttributeMetadata = entityMetadata.Attributes[i];
        if (success) break;
        if (AttributeMetadata.SchemaName.toLowerCase() == RetrieveAttributeName.toLowerCase()) {
            for (var o = 0; o < AttributeMetadata.OptionSet.Options.length; o++) {
                var option = AttributeMetadata.OptionSet.Options[o];
                if (option.OptionMetadata != null && option.OptionMetadata.Value == OptionValue) {
                    AssignAttributeName.innerText = option.OptionMetadata.Label.UserLocalizedLabel.Label;
                    success = true;
                    break;
                }
                if (option.StatusOptionMetadata != null && option.StatusOptionMetadata.Value == OptionValue) {
                    AssignAttributeName.innerText = option.StatusOptionMetadata.Label.UserLocalizedLabel.Label;
                    success = true;
                    break;
                }
            }
        }

    }


}

function errorDisplay(XmlHttpRequest, textStatus, errorThrown) {

    alert(errorThrown);
}

//************************************************************************************************************
//**************************************** By Ali Diab******************************************************
function CheckPhoneControl(cName) {
    var control = document.getElementById(cName);
    //control.title = 'ملحوظة:رقم التليفون أو الفاكس يجب أن يبدأ بصفر  ';
    control.onchange = function () {
        var check = checkIsPhone(control.value);
        if (!check) {
            alert("من فضلك قم بإدخال رقم هاتف صحيح.مثال" + '\n' + "05xxxxxxxx - 02xxxxxxxx");
            control.value = '';
            control.focus();
        }
    }
}
function checkIsPhone(phonenumber) {
    var IsPhone = false;
    //var pattern = /^((00|\+)966)\d{8,9}$/;
    var pattern = /^((0))\d{8,10}$/;

    if (pattern.test(phonenumber)) {
        IsPhone = true;
    }
    else {
        IsPhone = false;
    }
    return IsPhone;
}


function ValidateMobile(cName) {

    if (GetValueAttribute(cName.toString()) != null) {
        console.log("in=" + GetValueAttribute(cName.toString()));
        var check = IsMobile(GetValueAttribute(cName.toString()));
        if (!check) {
            alert("من فضلك قم بإدخال رقم جوال صحيح.مثال" + '\n' + "05xxxxxxxx");
            SetValueAttribute(cName.toString(), null);
        }
    }
}

function CheckMobileControl(cName) {

    BindOnChange(cName, function () { ValidateMobile(cName); });

}





function IsMobile(MobileNumber) {
    var IsPhone = false;
    //var pattern = /^((00|\+)966)\d{8,9}$/;
    var pattern = /^((0))\d{9}$/;

    if (pattern.test(MobileNumber)) {
        IsPhone = true;
    }
    else {
        IsPhone = false;
    }
    return IsPhone;
}
/// التحقق من ان المدخلات أرقام فقط
function Fun_NumbersOnly(ControlId) {
    //var pattern = /[0-9]+/g;
    var pattern = new RegExp("^[-]?[0-9]+[\.]?[0-9]+$");
    var control = document.getElementById(ControlId);
    control.onchange = function () {
        if (!pattern.test(control.value)) {
            alert("رقم الهاتف لا يقبل سوى أرقام");
            control.value = '';
            control.focus();
        }
    }
}

///وضع  "/" أثناء كتابة التاريخ
function SetDateSeparator(FieldName) {
    var Ctrl = document.getElementById(FieldName);
    Ctrl.onkeydown = function (e) {
        e = e || window.event;
        var charCode = e.which || e.keyCode;
        if (charCode == 111 || charCode == 191) {
            return false;
        }
    }
    Ctrl.onkeyup = function (evt) {
        evt = evt || window.event;
        var charCode = evt.which || evt.keyCode;
        if (charCode != 8 && charCode != 17) {
            var len = Ctrl.value.length;
            if (len == 2 || len == 5) {
                Ctrl.value = Ctrl.value + "/";
            }
        }
    };
}

function SetDateSeperatorForCalender(controlName) {
    var parentControl = document.getElementById(controlName);
    var Ctrl = parentControl.getElementsByTagName("input")[0];

    Ctrl.onkeydown = function (e) {
        e = e || window.event;
        var charCode = e.which || e.keyCode;
        if (charCode == 111 || charCode == 191) {
            return false;
        }
    }
    Ctrl.onkeyup = function (evt) {
        evt = evt || window.event;
        var charCode = evt.which || evt.keyCode;
        if (charCode != 8 && charCode != 17) {
            var len = Ctrl.value.length;
            if (len == 2 || len == 5) {
                Ctrl.value = Ctrl.value + "/";
            }
        }
    };
}
///وضع  "/" أثناء كتابة التاريخ
function SetDateSeparator2() {
    var Ctrl = GetAttribute('DateInput');
    Ctrl.onkeydown = function (e) {
        alert('test');
    };
}

function SaveForm() {
    Xrm.Page.data.entity.save();

}

//function ChangeFieldBackGround(fieldname, color) {

//    //crmForm.all[fieldname].style.backgroundColor = color;
//    var control = document.getElementById(fieldname);
//    if (control == null)
//        return;

//    var controles = document.getElementById(fieldname).parentNode.parentNode.getElementsByTagName('input');
//    for (var i = 0; i < controles.length; i++) {
//        controles[i].style.backgroundColor = color;
//        controles[i].style.color = 'blue';
//        controles[i].style.fontWeight = 'bold';
//    }
//    controles = document.getElementById(fieldname).parentNode.parentNode.getElementsByTagName('select');
//    for (var i = 0; i < controles.length; i++) {
//        controles[i].style.backgroundColor = color;
//        controles[i].style.color = 'blue';
//        //   controles[i].style.fontWeight='bold';
//    }
//    //SetEnabledState(fieldname, false);
//}

function ChangeFieldBackGround(fieldname, color) {

    //crmForm.all[fieldname].style.backgroundColor = color;
    var control = document.getElementById(fieldname);
    if (control == null)
        return;
    //new_profit_c
    //new_profit_d
    var cellLabel = document.getElementById(fieldname);
    if (cellLabel != null) {
        cellLabel.style.backgroundColor = color;
        cellLabel.style.color = 'blue';
        cellLabel.style.fontWeight = 'bold';
    }
    var cellData = document.getElementById(fieldname + "_c");
    if (cellData != null) {
        cellData.style.backgroundColor = color;
        cellData.style.color = 'blue';
        cellData.style.fontWeight = 'bold';
    }

    //SetEnabledState(fieldname, false);
}

function ChangeFieldBackGround2(fieldname, color) {
    crmForm.all[fieldname].style.backgroundColor = color;
}

function CheckDateFormat(control) {
    var minYear = 0001;
    var maxYear = (new Date()).getFullYear() - 575;
    var pattern = /^(\d{1,2})\/(\d{1,2})\/(\d{4})$/;
    var errorMsg = "";
    var field = document.getElementById(control);
    field.onchange = function () {
        if (field.value != '') {
            if (regs = field.value.match(pattern)) {
                if (regs[1] < 1 || regs[1] > 31) {
                    errorMsg = "خطأ قيمة اليوم هذه: " + regs[1];
                }
                else if (regs[2] < 1 || regs[2] > 12) {
                    errorMsg = "خطأقيمة الشهر هذه: " + regs[2];
                }
                else if (regs[3] < minYear) {
                    errorMsg = "خطأ قيمة السنة هذه: " + regs[3]; // + " - must be between " + minYear + " and " + maxYear;
                }
                else {
                    errorMsg = "";
                }
            }
            else {
                errorMsg = "هذا التاريخ خاطى يرجى كتابته على شكل :يوم/شهر/ سنة ";
            }
        }
        if (errorMsg != "") {
            alert(errorMsg);
            field.value = '';
            field.focus();
            return false;
        }
    }
}

///انشاء مربع نص التحويلة
function CreatPhoneExtension(control) {
    var field = document.getElementById(control);
    var PhoneID = field.id;
    if (document.getElementById(PhoneID) != null) {
        field.style.width = '65%'

        var labelTag = document.createElement("label");
        labelTag.id = "lbl" + PhoneID;
        labelTag.innerHTML = "تحويلة ";
        labelTag.style.width = '25px'

        var btnExtension = document.createElement('input');
        var Ext_Id = PhoneID + 'Ext';
        btnExtension.setAttribute('id', Ext_Id);
        btnExtension.setAttribute('type', 'text');
        btnExtension.title = 'التحويلة';
        btnExtension.style.width = '23%'
        field.parentNode.appendChild(labelTag);
        field.parentNode.appendChild(btnExtension);
        field.parentNode.setAttribute('nowrap', 'nowrap');
    }
}

//// Date Compare
var DatesCompare = function (a, b) {
    return (
        isFinite(a = this.Convertdate(a).valueOf()) &&
            isFinite(b = this.Convertdate(b).valueOf()) ?
            (a > b) - (a < b) :
            NaN
    );
}

function Convertdate(d) {
    return (
        d.constructor === Date ? d :
            d.constructor === Array ? new Date(d[0], d[1], d[2]) :
                d.constructor === Number ? new Date(d) :
                    d.constructor === String ? new Date(d) :
                        typeof d === "object" ? new Date(d.year, d.month, d.date) :
                            NaN
    );
}

////  مقارنة التاريخ المدخل مع تاريخ اليوم
function CompareDatewithToday(FieldName) {
    var FieldDate = GetValueAttribute(FieldName);
    var today = new Date();
    var result = DatesCompare(FieldDate.getDate(), today.getDate());
    if (result < 0) {
        alert("لا يمكن لهذا التاريخ أن يكون قبل تاريخ اليوم");
        SetValueAttribute(FieldName, null);
        SetFocus(FieldName);
    }
}
//// مقارنة تاريخ البداية وتاريخ النهاية
function CompareStartEndDates(StartDate, EndDate) {
    var start_date = GetValueAttribute(StartDate);
    var end_date = GetValueAttribute(EndDate);
    if (start_date != null && end_date != null) {
        //var result = DatesCompare(start_date.getDate(), end_date.getDate());
        //if (result >= 0) {
        //alert("لا يمكن لتاريخ الإصدارأن يساوى أو أكبر من تاريخ الإنتهاء");
        //SetValueAttribute(EndDate, null);
        //SetFocus(EndDate);
        //}
        if (Date.parse(start_date) > Date.parse(end_date)) {
            alert("لا يمكن لتاريخ الإصدارأن يساوى أو أكبر من تاريخ الإنتهاء");
            SetValueAttribute(EndDate, null);
            SetFocus(EndDate);
        }
    }
}

////حروف عربية فقط
function ArabicCharOnly(FieldName) {
    var Control = document.getElementById(FieldName);
    Control.title = 'أدخل اسماء بالعربية فقط';  //ToolTip
    var pattern = new RegExp("[a-zA-Z]+$");
    var reg = /^[-]?[0-9]+[\.]?[0-9]+$/;
    Control.onkeypress = function (evt) {
        evt = evt || window.event;
        var charCode = evt.which || evt.keyCode;
        var Ch = String.fromCharCode(charCode);
        if (pattern.test(Ch)) {
            alert("فضلا أدخل حروف عربية فقط");
            return false;
        }
    };
}

////حروف انجليزية فقط
function EnglishCharOnly(FieldName) {
    var Control = document.getElementById(FieldName);
    Control.title = 'فضلا أدخل حروف انجليزية فقط';  //ToolTip
    var pattern = new RegExp("[A-Za-z0-9 ._]");
    var reg = /^[-]?[0-9]+[\.]?[0-9]+$/;
    Control.onkeypress = function (evt) {
        evt = evt || window.event;
        var charCode = evt.which || evt.keyCode;
        var Ch = String.fromCharCode(charCode);
        if (!pattern.test(Ch)) {
            alert("فضلا أدخل حروف انجليزية فقط");
            return false;
        }
    };
}

///.......... Fill Lookup By Loggedin User ..............

function FillLookupByLoggedinUser(lookupId) {
    var curUserId = Xrm.Page.context.getUserId();
    var results = GetEntityByGuid('SystemUser', 'SystemUserId', curUserId)
    if (results != null) {
        var userName = results.results[0].FullName;
        SetLookupValue(lookupId, curUserId, userName, 'SystemUser');
    }
}
///.......... Fill Branch Lookup By Loggedin User ..............

function FillBranchLookup(lookupId) {
    var curUserId = Xrm.Page.context.getUserId();
    var results = GetEntityByGuid('SystemUser', 'SystemUserId', curUserId)
    if (results != null) {
        var BranchId = results.results[0].BusinessUnitId.Id;
        var BranchName = results.results[0].BusinessUnitId.Name;
        if (BranchId != null) {
            if (getLookUpValue(lookupId) == null) {
                SetLookupValue(lookupId, BranchId, BranchName, 'BusinessUnit');
            }
        }
    }
}
///..................................... Set Section Fields values to Null ..........................................

function SetSectionFieldsNull(sectionName) {
    Xrm.Page.ui.controls.forEach(function (control, index) {
        if (control.getParent().getName() == sectionName) {
            if (control.getName().indexOf('note') <= 0) {
                SetValueAttribute(control.getName(), null);
            }
        }
    });
}
///..................................... Disable LookupField Links ..........................................
function DisableLookup(FieldName) {
    var lookupParentNode = document.getElementById(FieldName + "_d");
    var lookupSpanNodes = lookupParentNode.getElementsByTagName("SPAN");

    for (var spanIndex = 0; spanIndex < lookupSpanNodes.length; spanIndex++) {
        var currentSpan = lookupSpanNodes[spanIndex];
        // Hide the hyperlink formatting
        currentSpan.style.textDecoration = "none";
        currentSpan.style.color = "#000000";
        // Revoke click functionality
        currentSpan.onclick = function () { };
    }
}
//************************************************************************************************************
//************************************************************************************************************
function EntityReference(id, name, entityLogicalName) {

    return { Id: id, LogicalName: entityLogicalName, Name: name };
}
function OptionSet(numberValue) {
    return { Value: Number(numberValue) }
}
function Money(strValue) {
    var Revenue = { Value: strValue };
    return Revenue;
}
function CreateRecord(entityNameDBCase, Record) {
    var ODataPath = getServerurl() + "/XRMServices/2011/OrganizationData.svc";

    var createNewAccount = new XMLHttpRequest();
    createNewAccount.open("POST", ODataPath + "/" + entityNameDBCase + "Set", false);
    createNewAccount.setRequestHeader("Accept", "application/json");
    createNewAccount.setRequestHeader("Content-Type", "application/json; charset=utf-8");

    createNewAccount.send(JSON.stringify(Record));
    var createAccountReturnValue = JSON.parse(createNewAccount.responseText).d;
    return createAccountReturnValue;

}

function UpdateRecordString(entityNameDbCase, id, fieldName, newValue) {
    var Record = {};
    eval('Record.' + fieldName + '="' + newValue + '";');
    UpdateRecord(entityNameDbCase, Record, id);
}
function UpdateRecord(entityNameDbCase, Record, id) {
    var ODataPath = getServerurl() + "/XRMServices/2011/OrganizationData.svc";
    //var ODataPath = "http;//crm1:5555/osoulmodern/XRMServices/2011/OrganizationData.svc";
    var req = new XMLHttpRequest();
    req.open("POST", ODataPath + "/" + entityNameDbCase + "Set(guid'" + id + "')", false);
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.setRequestHeader("X-HTTP-Method", "MERGE");
    req.send(JSON.stringify(Record));
    return req.responseText;

}

///********************** User Roles **************************************************************************************
function IsUserInRole(RoleId) {
    var UserRoles = Xrm.Page.context.getUserRoles();

    for (var i = 0; i < UserRoles.length; i++) {
        var Role = UserRoles[i];

        if (GuidsAreEqual(Role, RoleId)) {
            return true;
        }
    }
    return false;
}

function GuidsAreEqual(guid1, guid2) {
    var isEqual = false;

    if (guid1 == null || guid2 == null) {
        isEqual = false;
    }
    else {
        isEqual = guid1.replace(/[{}]/g, "").toLowerCase() == guid2.replace(/[{}]/g, "").toLowerCase();
    }
    return isEqual;
}
///********************** Current User ID **************************************************************************************
function CurrentUserID() {
    var CurUserId = Xrm.Page.context.getUserId();
    return CurUserId;
}

///..................................إنشاء دفعة.................................

function CreatNewPayment(AccountId, PaymentType, Amount, CreditNoteId, PaymentMethod) {

    var PaymentRecord = {};
    if (PaymentType != null) {
        PaymentRecord.new_PaymentType = { Value: Number(PaymentType) };
    }
    else {
        PaymentRecord.new_PaymentType = { Value: 1 };
    }
    if (PaymentMethod != null) {
        PaymentRecord.new_PaymentMethod = OptionSet(PaymentMethod);
    }
    else {
        PaymentRecord.new_PaymentMethod = OptionSet('100000000');
    }

    if (Amount != null) {
        PaymentRecord.new_Amount = { Value: Amount };
    }

    if (AccountId != null) {
        PaymentRecord.new_accountid = EntityReference(AccountId, '', 'account');
    }

    if (CreditNoteId != null) {
        PaymentRecord.new_CreditNoteId = EntityReference(CreditNoteId, '', 'new_creditnote');
    }

    //  PaymentRecord.new_PaymentMethod = { Value: 100000000 };
    ///............................................Create..............................................................
    var RetPayment = CreateRecord('new_installmentpayment', PaymentRecord);
    return RetPayment;
}

function ConvertStrToDate(fieldValue) {
    var strDate = fieldValue.substring(fieldValue.indexOf('(') + 1, fieldValue.indexOf(')'));
    var date = new Date(Number(strDate));
    return date;
}


function updateFetchXml(fetchxml, field, id) {
    var offset = 19;
    var fi = fetchxml.indexOf('<condition attribute="' + field + '" operator="eq" ');
    var nfetchxml = '';
    var closings = '';
    if (fi >= 0) {
        nfetchxml = fetchxml.substring(0, fi);
        offset = fetchxml.indexOf('/>', fi) - fi + 2;
    } else {
        fi = fetchxml.indexOf('<filter type="and">');
        if (fi < 0) {
            offset = 0;
            fi = fetchxml.indexOf('<entity');
            fi = fetchxml.indexOf('/>', fi) + 2;
            closings = '</filter>';
        }
        nfetchxml = fetchxml.substring(0, fi);
        nfetchxml += '<filter type="and">';
    }

    nfetchxml += '<condition attribute="' + field + '" ';
    nfetchxml += 'operator="eq" value="' + id + '" />';
    nfetchxml += closings;
    nfetchxml += fetchxml.substring(fi + offset);
    return nfetchxml;
}
function addFetchXmlcondition(fetchxml, condition) {
    var offset = 19;
    var fi = fetchxml.indexOf(condition);
    var nfetchxml = '';
    var closings = '';
    if (fi >= 0) {
        nfetchxml = fetchxml.substring(0, fi);
        offset = fetchxml.indexOf('/>', fi) - fi + 2;
    } else {
        fi = fetchxml.indexOf('<filter type="and">');
        if (fi < 0) {
            offset = 0;
            fi = fetchxml.indexOf('<entity');
            fi = fetchxml.indexOf('/>', fi) + 2;
            closings = '</filter>';
        }
        nfetchxml = fetchxml.substring(0, fi);
        nfetchxml += '<filter type="and">';
    }

    nfetchxml += condition;
    nfetchxml += closings;
    nfetchxml += fetchxml.substring(fi + offset);
    return nfetchxml;
}

function s4() {
    return Math.floor((1 + Math.random()) * 0x10000)
        .toString(16)
        .substring(1);
}

function guid() {
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
        s4() + '-' + s4() + s4() + s4();
}
function FilterLookUp(controlName, entity, curformFields, filterFieldsName) {
    var formFields = curformFields.split(',');
    var defaultViewId = Xrm.Page.getControl(controlName).getDefaultView();
    var viewEntity = GetEntityByGuid('SavedQuery', 'SavedQueryId', defaultViewId).results[0];
    var filterData = function () {
        //get defaul view id


        //Random Guid
        var viewId = '{1DFB2B35-B07C-44D1-868D-258DEEAB88E2}';
        var fetchXml = viewEntity.FetchXml;

        var filterFields = filterFieldsName.split(',');
        for (var i = 0; i < formFields.length; i++) {
            try {
                debugger;
                if (getLookUpValue(formFields[i]) != null) {
                    if (getLookUpValue(formFields[i]) != null && getLookUpValue(formFields[i]) != 'undefinded') {
                        fetchXml = updateFetchXml(fetchXml, filterFields[i], getLookUpValue(formFields[i]));
                    }
                }
            }
            catch (e) {
                if (GetValueAttribute(formFields[i]) != null) {
                    if (GetValueAttribute(formFields[i]) != null && GetValueAttribute(formFields[i]) != 'undefinded') {
                        fetchXml = updateFetchXml(fetchXml, filterFields[i], GetValueAttribute(formFields[i]));
                    }
                }

            }

        }
        //alert(fetchXml);

        var LayoutXml = viewEntity.LayoutXml;
        LayoutXml = LayoutXml.replace('icon="1"', 'icon="0"');
        LayoutXml = LayoutXml.replace('preview="1"', 'preview="0"');
        //var Entity = viewEntity.entity;
        var QueryId = viewId;
        var viewDisplayName = "Filter View";
        Xrm.Page.getControl(controlName).addCustomView(viewId, entity, viewDisplayName, fetchXml, LayoutXml, true);
    }
    for (var j = 0; j < formFields.length; j++) {
        BindOnChange(formFields[j], filterData);
    }
    filterData();
}

function FilterLookUpWithCondition(controlName, entity, curformFields, filterFieldsName, condition) {

    var defaultViewId = Xrm.Page.getControl(controlName).getDefaultView();
    var viewEntity = GetEntityByGuid('SavedQuery', 'SavedQueryId', defaultViewId).results[0];
    var formFields = null;
    var filterData = function () {
        //get defaul view id


        //Random Guid
        var viewId = '{1DFB2B35-B07C-44D1-868D-258DEEAB88E2}';
        var fetchXml = viewEntity.FetchXml;
        if (condition != null) {
            fetchXml = addFetchXmlcondition(fetchXml, condition);
        }
        if (filterFieldsName != null && filterFieldsName != "") {
            var filterFields = filterFieldsName.split(',');
            formFields = curformFields.split(',');
            for (var i = 0; i < formFields.length; i++) {
                /*
                if (Xrm.Page.getControl(formFields[i]) != 'lookup') {
                if (GetValueAttribute(formFields[i]) != null && GetValueAttribute(formFields[i]) != 'undefinded') {
                fetchXml = updateFetchXml(fetchXml, filterFields[i], GetValueAttribute(formFields[i]));
                }
                }
                else
                */
                {
                    if (getLookUpValue(formFields[i]) != null && getLookUpValue(formFields[i]) != 'undefinded') {
                        fetchXml = updateFetchXml(fetchXml, filterFields[i], getLookUpValue(formFields[i]));
                    }
                }
            }
        }
        //alert(fetchXml);

        var LayoutXml = viewEntity.LayoutXml;
        //var Entity = viewEntity.entity;
        var QueryId = viewId;
        var viewDisplayName = "Filter View";
        Xrm.Page.getControl(controlName).addCustomView(viewId, entity, viewDisplayName, fetchXml, LayoutXml, true);
    }
    if (formFields != null) {
        for (var j = 0; j < formFields.length; j++) {
            BindOnChange(formFields[j], filterData);
        }
    }
    filterData();
}

function SetDefault(fieldName, value) {
    if (GetValueAttribute(fieldName) == null || GetValueAttribute(fieldName) == 'undefined') {
        SetValueAttribute(fieldName, value);
    }
}
function SetDefaultOrZero(fieldName, value) {
    if (GetValueAttribute(fieldName) == null || GetValueAttribute(fieldName) == 'undefined' || GetValueAttribute(fieldName) == 0) {
        SetValueAttribute(fieldName, value);
    }
}
function OpenPopUpWindow(url) {
    var strWindowFeatures = "menubar=yes,location=yes,resizable=yes,scrollbars=yes,status=yes";
    windowObjectReference = window.open(url, "CNN_WindowName", strWindowFeatures);
}
function getISVWebUrl() {
    var orgName = Xrm.Page.context.getOrgUniqueName();
    //if (orgName.toLowerCase().indexOf("test") != -1)
    // return "http://" + window.location.hostname + ":9000/";
    // else
    return "https://" + window.location.hostname + ":8001/";
}

function SetLookUpResult(formField, resultDBField, resultRow) {
    var fieldVal = eval('resultRow.' + resultDBField);
    if (fieldVal.Id != null) {
        SetLookupValue(formField, fieldVal.Id, fieldVal.Name, fieldVal.LogicalName);
    }
}


function BindCheckWithDate(checkFieldName, checkValue, dateFieldName, optionsetFieldName, optionValue, previousValue) {

    BindOnChange(checkFieldName, function () {
        if (GetValueAttribute(checkFieldName) == checkValue) {
            if (dateFieldName != '')
                SetValueAttribute(dateFieldName, new Date());
            SetValueAttribute(optionsetFieldName, optionValue);

        }
        else {
            if (dateFieldName != '')
                SetValueAttribute(dateFieldName, null);
            if (previousValue != null)
                SetValueAttribute(optionsetFieldName, previousValue);
            else
                SetValueAttribute(optionsetFieldName, 1);

        }
    });
}

function checkUniqueness(entityName, fieldName, displayName) {

    BindOnChange(fieldName.toString().toLowerCase(), function () {
        var results = GetEntityByString(entityName, fieldName, GetValueAttribute(fieldName.toString().toLowerCase())).results;
        if (IsCreateForm()) {
            if (results != null && results.length > 0) {
                alert(displayName + " is already exist. Please inserted a different value");
                SetValueAttribute(fieldName.toString().toLowerCase(), null);
            }
        }
        else if (results != null && results.length > 1) {
            alert(displayName + " is already exist. Please inserte a different value");
            SetValueAttribute(fieldName.toString().toLowerCase(), null);
        }
    });
}

function checkUniquenessAjax(tableName, fieldName, curAttributeName, displayName) {

    BindOnChange(curAttributeName.toString().toLowerCase(), function () {

        //  if (IsCreateForm()) 
        {
            var value = GetValueAttribute(curAttributeName.toString().toLowerCase());
            var url = getServerurl() + "/ISV/CheckValueExist.ashx?tablename=" + tableName + "&fieldname=" + fieldName + "&fieldvalue=" + value;
            var count = callAjax(url)

            if (count != null && Number(count) > 0) {
                SetValueAttribute(curAttributeName.toString().toLowerCase(), null);
                alert(" يوجد سجل آخر للحقل" + " " + displayName + " بنفس قيمة " + value);
                // return true;
            }


        }

    });
}
function checkUniquenessAjaxList(tableName, columns, fields, displayName) {

    //var value = GetValueAttribute(curAttributeName.toString().toLowerCase());
    var fieldArray = fields.split(',');
    var values = "";
    var value = null;
    for (i = 0; i < fieldArray.length; i++) {

        if (IsLookUp(fieldArray[i])) {
            value = getLookUpValue(fieldArray[i]);
        }
        else {

            value = GetValueAttribute(fieldArray[i]);
        }
        if (value == null)
            return;
        values += value;
        if (i < fieldArray.length - 1)
            values += ",";
    }
    var url = getServerurl() + "/ISV/CheckValueExist02.ashx?tablename=" + tableName + "&fieldname=" + columns + "&fieldvalue=" + values;
    var count = callAjax(url)

    if (count != null && Number(count) > 0) {
        for (var i = 0; i < fieldArray.length; i++) {
            SetValueAttribute(fieldArray[i], null);
            SetMandatory(fieldArray[i], true);
        }
        // disableFormFields(true);

        alert(" يوجد سجل آخر للحقول" + " " + displayName + " بنفس القيمة ");
        // return true;
    }
}
function ConvertDateToString(date) {
    //var date = new Date();
    var curr_date = date.getDate();
    curr_date = ((curr_date <= 9) ? "0" : "") + curr_date.toString();
    var curr_month = date.getMonth() + 1; //Months are zero based
    curr_month = ((curr_month <= 9) ? "0" : "") + curr_month.toString();
    var curr_year = date.getFullYear();
    return curr_date + '/' + curr_month + '/' + curr_year;
}
function bindHijriDate(hijriDateField, georgianDateField) {
    function OnChangeHijri() {

        var hijriDate = GetValueAttribute(hijriDateField);
        var results = GetEntityByString("new_thijridate", "new_hijridateText", hijriDate).results;
        if (results != null && results.length > 0) {
            //  var fieldValue = eval(results[0].new_georgiandate);
            var dateValue = ConvertStrToDate(results[0].new_georgiandate);
            SetValueAttribute(georgianDateField, dateValue);
        }

    }
    function OnChangeGeorgian() {

        var georgianDate = GetValueAttribute(georgianDateField);
        var geostring = ConvertDateToString(georgianDate)
        var results = GetEntityByString("new_thijridate", "new_georgiantxt", geostring).results;
        if (results != null && results.length > 0) {
            SetValueAttribute(hijriDateField, results[0].new_hijridateText);
        }
    }

    BindOnChange(hijriDateField, OnChangeHijri);
    BindOnChange(georgianDateField, OnChangeGeorgian);

}
function GetNextSequence(prefixSeq) {
    //var url = getServerurl() + "/ISV/GETSequence.ashx";
    //url += "?seq=" + prefixSeq;

    //var result = callAjax(url);
    //return result;
    return null;
}
function OpenRecord(entityName, recordId) {
    window.open("/main.aspx?etn=" + entityName + "&pagetype=entityrecord&id=%7B" + recordId + "%7D", recordId);
}
function checkAndOpen(formFieldDbCase, entityNameDbCase, targetField) {

    BindOnChange(formFieldDbCase.toString().toLowerCase(), function () {
        var value = GetValueAttribute(formFieldDbCase.toString().toLowerCase());
        // var url = getServerurl() + "/ISV/ReturnGuidExist.ashx?tablename=" + entityName + "&fieldname=" + formField + "&fieldvalue=" + value;
        // entityNameDbCase + "Id",
        var results = GetEntityByString(entityNameDbCase, formFieldDbCase, value).results;
        if (results != null && results.length > 0) {
            var guid = results[0].entityNameDbCase + "Id";
            if (guid != "" && guid != null) {
                OpenRecord(entityNameDbCase.toLowerCase(), guid);
            }
        }
    });
}
function removeMandatoryForRole(roleID) {
    var userInRole = IsUserInRole(roleID);
    if (userInRole) {
        //remove all
        removeMandatoryFormFields(true);
    }

}
function checkCurrentUserInTeam(teamId) {

    // var teamId = Xrm.Page.data.entity.attributes.get("new_teamid").getValue()[0].id;

    var userId = Xrm.Page.context.getUserId();
    //alert(userId);
    if (teamId != null) {

        var fwdFilter = "TeamMembershipSet?$filter=TeamId eq guid'" + teamId + "' and SystemUserId eq guid'" + userId + "'";
        var url = getServerurl() + "/xrmservices/2011/OrganizationData.svc/" + fwdFilter;
        var fwdResult = GetOdataResults(url).results;

        if (fwdResult != null && fwdResult.length > 0) {
            return true;

        }
        else {
            return false;
        }

    }
    return false;

}

//Validate IQama and ID number
function CheckIdentity(IDNO) {
    var Result; // int
    var Type; //varchar(1);
    var i; //int;
    var ZFOdd; //varchar(max);
    var SubString; // varchar(1);
    var InTVALUE1; //int;
    var InTVALUE2; //int;
    var sum; //int;
    Result = 1;
    sum = 0;

    Type = IDNO.substr(0, 1);

    if (IDNO.length != 10) {
        return Result = 11;
    } else {

        if (Type != '1' && Type != '2') {
            return Result = 12;
        } else {
            var i = 0;
            while (i < 10) {
                if (i % 2 == 0) {
                    SubString = IDNO.substr(i, 1);
                    InTVALUE1 = parseInt(SubString) * 2;
                    if (InTVALUE1.toString().length == 1) {
                        ZFOdd = '0' + InTVALUE1.toString();
                    } else if (InTVALUE1.toString().length == 0) {
                        ZFOdd = '00';
                    } else {
                        ZFOdd = InTVALUE1.toString();
                    }
                    InTVALUE1 = parseInt(ZFOdd.substr(0, 1));
                    InTVALUE2 = parseInt(ZFOdd.substr(1, 1));
                    sum = sum + InTVALUE1 + InTVALUE2;

                } else {
                    sum = sum + parseInt(IDNO.substr(i, 1));

                }
                i = i + 1;
            } // while loop


        }
        Result = sum % 10;

    }
    return Result;
}

function checkId(idField) {
    var idnumber = GetValueAttribute(idField);
    if (CheckIdentity(idnumber) != 0) {
        alert("رقم الهوية غير صحيح");
        SetValueAttribute(idField, null);
    }

}

function checkIqamaNumber(idField) {

    BindOnChange(idField, function () { checkId(idField); });
}

function IsLookUp(controlName) {
    var type = Xrm.Page.getControl(controlName).getControlType();
    if (type == 'lookup') {
        return true;
    }
    else
        return false;
}
function IsNotLookUp(controlName) {
    var type = Xrm.Page.getControl(controlName).getControlType();
    if (type == 'lookup') {
        return false;
    }
    else
        return true;
}

function GetCurrentEntityName() {

    return Xrm.Page.data.entity.getEntityName();
}

function CheckStatusSecurity(fieldName, entityName, StatusValue, UserID) {

    var url = getServerurl() + "/ISV/ISVMawarid/GetTeamAndRole.ashx?userid=" + UserID + "&value=" + StatusValue + "&fieldName=" + fieldName + "&entityName=" + entityName;
    var Result = callAjax(url);

    return Result;

}

function OpenRecord(entityName, recordID) {
    Xrm.Utility.openEntityForm(entityName, recordID);
}

function OpenNewRecord(entityName) {
    Xrm.Utility.openEntityForm(entityName)
}

function OpenNewRecordParamters(entityName, fieldsComma, valuesComma) {

    var parameters = {};
    var fieldArray = fieldsComma.split(",");
    var valuesArray = valuesComma.split(",");
    if (fieldArray.length == valuesArray.length) {
        for (var i = 0; i < fieldArray.length; i++) {
            parameters[fieldArray[i]] = valuesArray[i];
        }


        //pop incident form with default values
        Xrm.Utility.openEntityForm(entityName, null, parameters);
    }
    else {
        alert("length is not same");
    }
}



function ValidateEmail(email) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    if (re.test(email)) {
        return true;
    } else {
        return false;
    }
}




function setMaxValue(fieldName, value) {
    var control = document.getElementById(fieldName);
    if (control != null) {
        control.setAttribute("max", value);
    }
}

function setMinValue(fieldName, value) {
    var control = document.getElementById(fieldName);
    if (control != null) {
        control.setAttribute("min", value);
    }
}
function CallISVResponseURL(callURL) {
    callURL = encodeURIComponent(callURL)
    var url = getServerurl() + "/ISV/GetUrlResponse.ashx?Link=" + callURL;

    var result = callAjax(url);
    return result;
}


function CheckNumberLength(fieldName, length) {

    var cid = GetValueAttribute(fieldName);
    var pattern = /[^0-9]/;

    if (pattern.test(cid)) {
        return false;
    } else if (cid.length < length || cid.length > length) {
        return false;
    }

    return true;

}

function GetIPAddress() {
    var url = getServerurl() + "/ISV/GetIPAddress.ashx";
    var result = callAjax(url);
    return result;
}

function preventAutoSave(econtext) {

    var eventArgs = econtext.getEventArgs();

    if (eventArgs.getSaveMode() == 70) {

        eventArgs.preventDefault();

    }

}


function addLookUpFilter(lookupName, fetchXMLFilter) {
    // add the event handler for PreSearch Event
    Xrm.Page.getControl(lookupName).addPreSearch(function () {

        Xrm.Page.getControl(lookupName).addCustomFilter(fetchXMLFilter);
    }

    );

}


function SetIframeSrc(frameName, url) {

    Xrm.Page.ui.controls.get(frameName).setSrc(url);
}


function GetCurrentUrlParamters() {

    var QueryString = "?id=" + GetFormID() + "&orglcid=1025&orgname=" + Xrm.Page.context.getOrgUniqueName() + "&typename=" + GetCurrentEntityName() + "&UID=" + CurrentUserID();

    return QueryString;
}

function OpenISVWindow(pagelocation) {
    var url = getISVWebUrl();
    url += pagelocation;
    url += GetCurrentUrlParamters();
    url += "&Time=" + new Date().getTime();
    url += "&Sign=" + GetSignature(url);
    openNewWindow(url);

}

function OpenGlobalISVWindow(hostandport, pagelocation) {
    var url = "https://" + hostandport;
    url += pagelocation;
    url += GetCurrentUrlParamters();
    url += "&Time=" + new Date().getTime();
    url += "&Sign=" + GetSignature(url);
    openNewWindow(url);

}


function OpenNewlyMuqeem() {
    OpenISVWindow("Emp/Muqueem.aspx");
} /// <reference path="XrmPage-vsdoc.js" />
/// <reference path="sdk.metadata.js" />

/**
* http://www.openjs.com/scripts/events/keyboard_shortcuts/
* Version : 1.00.A
* By Binny V A
* License : BSD
*/
function shortcut(shortcut, callback, opt) {
    //Provide a set of default options
    var default_options = {
        'type': 'keydown',
        'propagate': false,
        'target': document
    }
    if (!opt) opt = default_options;
    else {
        for (var dfo in default_options) {
            if (typeof opt[dfo] == 'undefined') opt[dfo] = default_options[dfo];
        }
    }
    var ele = opt.target
    if (typeof opt.target == 'string') ele = document.getElementById(opt.target);
    var ths = this;
    //The function to be called at keypress
    var func = function (e) {
        e = e || window.event;
        //Find Which key is pressed
        if (e.keyCode) code = e.keyCode;
        else if (e.which) code = e.which;
        var character = String.fromCharCode(code).toLowerCase();
        var keys = shortcut.toLowerCase().split("+");
        //Key Pressed - counts the number of valid keypresses - if it is same as the number of keys, the shortcut function is invoked
        var kp = 0;
        //Work around for stupid Shift key bug created by using lowercase - as a result the shift+num combination was broken
        var shift_nums = {
            "`": "~",
            "1": "!",
            "2": "@",
            "3": "#",
            "4": "$",
            "5": "%",
            "6": "^",
            "7": "&",
            "8": "*",
            "9": "(",
            "0": ")",
            "-": "_",
            "=": "+",
            ";": ":",
            "'": "\"",
            ",": "<",
            ".": ">",
            "/": "?",
            "\\": "|"
        }
        //Special Keys - and their codes
        var special_keys = {
            'esc': 27,
            'escape': 27,
            'tab': 9,
            'space': 32,
            'return': 13,
            'enter': 13,
            'backspace': 8,
            'scrolllock': 145,
            'scroll_lock': 145,
            'scroll': 145,
            'capslock': 20,
            'caps_lock': 20,
            'caps': 20,
            'numlock': 144,
            'num_lock': 144,
            'num': 144,
            'pause': 19,
            'break': 19,
            'insert': 45,
            'home': 36,
            'delete': 46,
            'end': 35,
            'pageup': 33,
            'page_up': 33,
            'pu': 33,
            'pagedown': 34,
            'page_down': 34,
            'pd': 34,
            'left': 37,
            'up': 38,
            'right': 39,
            'down': 40,
            'f1': 112,
            'f2': 113,
            'f3': 114,
            'f4': 115,
            'f5': 116,
            'f6': 117,
            'f7': 118,
            'f8': 119,
            'f9': 120,
            'f10': 121,
            'f11': 122,
            'f12': 123
        }
        for (var i = 0; k = keys[i], i < keys.length; i++) {
            //Modifiers
            if (k == 'ctrl' || k == 'control') {
                if (e.ctrlKey) kp++;
            } else if (k == 'shift') {
                if (e.shiftKey) kp++;
            } else if (k == 'alt') {
                if (e.altKey) kp++;
            } else if (k.length > 1) { //If it is a special key
                if (special_keys[k] == code) kp++;
            } else { //The special keys did not match
                if (character == k) kp++;
                else {
                    if (shift_nums[character] && e.shiftKey) { //Stupid Shift key bug created by using lowercase
                        character = shift_nums[character];
                        if (character == k) kp++;
                    }
                }
            }
        }
        if (kp == keys.length) {
            callback(e);
            if (!opt['propagate']) { //Stop the event
                //e.cancelBubble is supported by IE - this will kill the bubbling process.
                e.cancelBubble = true;
                e.returnValue = false;
                //e.stopPropagation works only in Firefox.
                if (e.stopPropagation) {
                    e.stopPropagation();
                    e.preventDefault();
                }
                return false;
            }
        }
    }
    //Attach the function with the event
    if (ele.addEventListener) ele.addEventListener(opt['type'], func, false);
    else if (ele.attachEvent) ele.attachEvent('on' + opt['type'], func);
    else ele['on' + opt['type']] = func;
}

shortcut('Ctrl+Q', CollapseAll);
shortcut('Ctrl+W', ExpandAll);
var parentOptions = {

    'target': document.parentDocument
};
shortcut('Ctrl+Q', CollapseAll, parentOptions);
shortcut('Ctrl+W', ExpandAll, parentOptions);

String.prototype.CustomReplace = function (value, valuetoReplace) {
    var x = this.replace(value, valuetoReplace);
    if (x.indexOf(value) >= 0)
        x = x.CustomReplace(value, valuetoReplace);
    return x;
}


function SetLookupValue(fieldName, id, name, entityType) {
    if (fieldName != null) {
        if (id == null || name == null) {
            SetValueAttribute(fieldName, null);
            return;
        }
        var lookupValue = new Array();
        lookupValue[0] = new Object();
        lookupValue[0].id = id;
        lookupValue[0].name = name;
        lookupValue[0].entityType = entityType;
        Xrm.Page.getAttribute(fieldName).setValue(lookupValue);
    }


}

function GetFormID() {
    var GUIDvalue = Xrm.Page.data.entity.getId();
    return GUIDvalue;
}
function RefreshGrid(gridName) {
    var targetgird = Xrm.Page.ui.controls.get(gridName);
    targetgird.Refresh();
}
function GetEntityByID(entityName, fields, id) {

    var arrvalue = new Array();
    var url = 'https://' + window.location.hostname + ":8000/GetEntityDataById.ashx?Id=" + id + "&Entity=" + entityName + "&Columns=" + fields;
    url += '&time=' + new Date().getTime();
    str = callAjax(url);
    eval(str);
    return arrvalue;
}
function GetEntityByGuid(entityName, searchByColumn, searchByValue) {


    //var serverUrl = Xrm.Page.context.getServerUrl();
    var url = getServerurl() + "/xrmservices/2011/OrganizationData.svc/" + entityName + "Set?$filter=" + searchByColumn + " eq guid'" + searchByValue + "'";
    str = CallOData(url);
    var data = eval('(' + str + ')');
    return data.d;
}
function SelectEntityByGuid(entityName, columns, searchByColumn, searchByValue) {


    //var serverUrl = Xrm.Page.context.getServerUrl();
    var url = getServerurl() + "/xrmservices/2011/OrganizationData.svc/" + entityName + "Set?$select=" + columns + "&$filter=" + searchByColumn + " eq guid'" + searchByValue + "'";
    str = CallOData(url);
    var data = eval('(' + str + ')');
    return data.d;
}
function getServerurl() {
    var serverUrl = "https://" + window.location.hostname + (window.location.port ? ':' + window.location.port : '') + "/" + Xrm.Page.context.getOrgUniqueName();
    return serverUrl;
}
function GetEntityByRefrenceID(entityName, searchByColumn, searchByValue) {


    //var serverUrl = Xrm.Page.context.getServerUrl();
    var url = getServerurl() + "/xrmservices/2011/OrganizationData.svc/" + entityName + "Set?$filter=" + searchByColumn + "/Id  eq guid'" + searchByValue + "'";
    str = CallOData(url);
    var data = eval('(' + str + ')');
    return data.d;
}
function SelectEntityByRefrenceID(entityName, columns, searchByColumn, searchByValue) {


    //var serverUrl = Xrm.Page.context.getServerUrl();
    var url = getServerurl() + "/xrmservices/2011/OrganizationData.svc/" + entityName + "Set?$select=" + columns + "&$filter=" + searchByColumn + "/Id  eq guid'" + searchByValue + "'";
    str = CallOData(url);
    var data = eval('(' + str + ')');
    return data.d;
}
function GetEntityByString(entityName, searchByColumn, searchByValue) {


    //var serverUrl = Xrm.Page.context.getServerUrl();
    var url = getServerurl() + "/xrmservices/2011/OrganizationData.svc/" + entityName + "Set?$filter=" + searchByColumn + " eq '" + searchByValue + "'";
    str = CallOData(url);
    var data = eval('(' + str + ')');
    return data.d;
}
//
function SelectEntityByString(entityName, columns, searchByColumn, searchByValue) {


    //var serverUrl = Xrm.Page.context.getServerUrl();
    var url = getServerurl() + "/xrmservices/2011/OrganizationData.svc/" + entityName + "Set?$select=" + columns + "&$filter=" + searchByColumn + " eq '" + searchByValue + "'";
    str = CallOData(url);
    var data = eval('(' + str + ')');
    return data.d;
}
function GetEntityByDate(entityName, searchByColumn, searchByValue) {


    //var serverUrl = Xrm.Page.context.getServerUrl();
    var url = getServerurl() + "/xrmservices/2011/OrganizationData.svc/" + entityName + "Set?$filter=" + searchByColumn + " eq datetime'" + searchByValue + "'";
    str = CallOData(url);
    var data = eval('(' + str + ')');
    return data.d;
}

function GetEntityByNumber(entityName, searchByColumn, searchByValue) {


    //var serverUrl = Xrm.Page.context.getServerUrl();
    var url = getServerurl() + "/xrmservices/2011/OrganizationData.svc/" + entityName + "Set?$filter=" + searchByColumn + " eq " + searchByValue + "";
    str = CallOData(url);
    var data = eval('(' + str + ')');
    return data.d;
}

function callAjax(url) {
    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    }
    else {// code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    xmlhttp.open("GET", url, false);
    xmlhttp.setRequestHeader("X-Requested-Width", "XMLHttpRequest");
    xmlhttp.setRequestHeader("Accept", "application/json, text/javascript, */*");
    xmlhttp.send(null);

    return xmlhttp.responseText;

}

function CallOData(url) {
    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    }
    else {// code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    xmlhttp.open("GET", url, false);
    xmlhttp.setRequestHeader("X-Requested-Width", "XMLHttpRequest");
    xmlhttp.setRequestHeader("Accept", "application/json, text/javascript, */*");
    xmlhttp.send(null);
    return xmlhttp.responseText;

}
function GetOdataResults(url) {
    CallOData(url);
    str = CallOData(url);
    var data = eval('(' + str + ')');
    return data.d;
}

function SetEnabledState(controlName, value) {
    var AddressType = Xrm.Page.ui.controls.get(controlName);
    AddressType.setDisabled(value);
}


function setEnabled(controlName) {
    var AddressType = Xrm.Page.ui.controls.get(controlName);
    AddressType.setDisabled(false);
}
function setDisabled(controlName) {
    var AddressType = Xrm.Page.ui.controls.get(controlName);
    AddressType.setDisabled(true);
}
function SetDisabledWithSaving(controlName) {
    setDisabled(controlName)
    Xrm.Page.getAttribute(controlName).setSubmitMode("always");
}

function getLookUpValue(controlName) {
    var ExistingCase = Xrm.Page.data.entity.attributes.get(controlName);
    if (ExistingCase.getValue() == null) { return null; };
    var ExistingCaseGUID = ExistingCase.getValue()[0].id;
    return ExistingCaseGUID;
}
function getLookUpDisplay(controlName) {

    var ExistingCase = Xrm.Page.data.entity.attributes.get(controlName);
    if (ExistingCase.getValue() == null) { return null; };
    var ExistingCaseGUID = ExistingCase.getValue()[0].name;
    return ExistingCaseGUID;
}
function onCustomerChange() {

    var id = Xrm.Page.data.entity.attributes.get("new_customeraccount").getValue()[0].id;
    var account = GetEntityByID('account', 'accountid,name,new_totalincome,new_totalcommitment,new_accountstype', id);
    crmForm.all.new_customeraverageincome.DataValue = checkNumber(account["new_totalincome"]);
    crmForm.all.new_customeravergaecommitment.DataValue = checkNumber(account["new_totalcommitment"]);

    crmForm.all.new_customeraccounttype.DataValue = checkNumber(account["new_accountstype"]);
    CustomerfinanceCalCulator();
}

function DisableSection(sectionName) {
    //disable all the controls that has section as parent
    Xrm.Page.ui.controls.forEach(function (control, index) {
        if (control.getParent() != null) {
            if (control.getParent().getName() == sectionName) {
                if (control.getControlType() != 'subgrid') {
                    //                if (control.getControlType() == 'lookup') {
                    //                    DisableLookup(control.getName());
                    //                }
                    control.setDisabled(true);
                }
            }
        }
    });
}

function SetSectionMandatory(sectionName) {
    Xrm.Page.ui.controls.forEach(function (control, index) {
        if (control.getParent().getName() == sectionName) {
            if (control.getName().indexOf('note') <= 0) {
                SetMandatory(control.getName());
            }

        }
    });
}

function RemoveSectionMandatory(sectionName) {
    Xrm.Page.ui.controls.forEach(function (control, index) {
        if (control.getParent() != null) {
            if (control.getParent().getName() == sectionName) {
                RemoveMandatory(control.getName());
            }
        }
    });
}
function onSponsorChange() {

    var id = Xrm.Page.data.entity.attributes.get("new_sponsor").getValue()[0].id;
    var account = GetEntityByID('account', 'accountid,name,new_totalincome,new_totalcommitment,new_accountstype', id);
    crmForm.all.new_sponsoraverageincome.DataValue = checkNumber(account["new_totalincome"]);
    crmForm.all.new_sponsoraveragecommitment.DataValue = checkNumber(account["new_totalcommitment"]);
    crmForm.all.new_sponsoraccounttype.DataValue = checkNumber(account["new_accountstype"]);
    initalizeForm();
    CustomerfinanceCalCulator();
}
function checkNumber(num) {
    if (num == null)
        return null;
    else {
        return Number(num);
    }
}

function HideShowSection(tabName, sectionName, visible) {

    Xrm.Page.ui.tabs.get(tabName).sections.get(sectionName).setVisible(visible);
}

function SetMandatory(fieldName) {
    if (Xrm.Page.getAttribute(fieldName) != null && Xrm.Page.getAttribute(fieldName).setRequiredLevel != null) {
        Xrm.Page.getAttribute(fieldName).setRequiredLevel("required");
    }
}
/*function setToggleMandatory(fieldName, valueForRequired, mandatoryFields) {

    BindOnChange(fieldName, function () {
        var fields = mandatoryFields.split(',');
        var fieldValue = GetValueAttribute(fieldName);
        for (i = 0; i < fields.length; i++) {
            if (fieldValue == valueForRequired)
                SetMandatory(fields[i]);
            else
                RemoveMandatory(fields[i]);
        }
    });
}*/
function SetDisableWhen(fieldName, valueForRequired, disableFields) {
    var fields = disableFields.split(',');
    var fieldValue = GetValueAttribute(fieldName);
    if (fieldValue == valueForRequired)
        SetEnabledState(fieldName, true);
    else
        SetEnabledState(fieldName, false);
    for (i = 0; i < fields.length; i++) {
        if (fieldValue == valueForRequired)
            SetEnabledState(fields[i], true);
        else
            SetEnabledState(fields[i], false);
    }
}
function SetEnableWhen(fieldName, valueForRequired, disableFields) {
    var fields = disableFields.split(',');
    var fieldValue = GetValueAttribute(fieldName);
    if (fieldValue == valueForRequired) {
        for (i = 0; i < fields.length; i++) {

            SetEnabledState(fields[i], false);
        }
    }
}
function setMandatory(fieldName, valueForRequired) {
    var fieldValue = GetValueAttribute(fieldName);
    if (valueForRequired == fieldValue)
        SetMandatory(fieldName);
    else
        RemoveMandatory(fieldName);
}
function RemoveMandatory(fieldName) {
    if (Xrm.Page.getAttribute(fieldName) != null && Xrm.Page.getAttribute(fieldName).setRequiredLevel != null) {
        Xrm.Page.getAttribute(fieldName).setRequiredLevel("none");
    }
}
function SetTabVisibility(tabName, value) {
    if (Xrm.Page.ui.tabs.get(tabName)) {
        Xrm.Page.ui.tabs.get(tabName).setVisible(value);
    }
    else {
        alert(tabName);
    }
}

function SetTabCollapse(tabName) {
    Xrm.Page.ui.tabs.get(tabName).setDisplayState('collapsed');
}
function SetTabExpand(tabName) {
    Xrm.Page.ui.tabs.get(tabName).setDisplayState('expanded');
}

function CollapseAll() {
    Xrm.Page.ui.tabs.forEach(
        function (control) {
            control.setDisplayState('collapsed');
        }
    );

}
function ExpandAll() {
    Xrm.Page.ui.tabs.forEach(
        function (control) {
            control.setDisplayState('expanded');
        }
    );
}


function SetSectionVisablity(sectionName, sectionIsVisible) {
    //Hide or Show Sections 
    var tabs = Xrm.Page.ui.tabs.get();
    for (var i in tabs) {
        var tab = tabs[i];
        tab.sections.forEach(function (section, index) {
            if (section.getName() == sectionName) {
                section.setVisible(sectionIsVisible);
            }
        });
    }
}
function GetSection(sectionName) {
    //Hide or Show Sections 
    var tabs = Xrm.Page.ui.tabs.get();
    for (var i in tabs) {
        var tab = tabs[i];
        tab.sections.forEach(function (section, index) {
            if (section.getName() == sectionName) {
                return section;
            }
        });
    }
}
function SetTabVisibilityWhen(tabName, boolVisibleValue, dependOnFieldName, values) {
    var fieldvalue = GetValueAttribute(dependOnFieldName);
    if (fieldvalue != null) {
        if (values.indexOf(fieldvalue) >= 0) {
            SetTabVisibility(tabName, boolVisibleValue);
        }
        else {
            SetTabVisibility(tabName, !boolVisibleValue);
        }
    }
    else {
        SetTabVisibility(tabName, !boolVisibleValue);
    }
}
function onPaymentMethod() {

    var value = Xrm.Page.data.entity.attributes.get("new_paymentmethod").getValue();
    if (value == null) return;
    var sections = new Array();
    sections['100000000'] = 'cash';
    sections['100000001'] = 'cheque';
    sections['100000002'] = 'transfer';
    sections['100000003'] = 'atm';
    for (key in sections) {
        if (key == value) {
            HideShowSection('GeneralTab', sections[key], true);
            //  SetSectionMandatory(sections[key]);

        }
        else {
            try {
                RemoveSectionMandatory(sections[key]);
            }
            catch (e) { }
            HideShowSection('GeneralTab', sections[key], false);

        }
    }

}
function GetAttribute(attributeName) {
    return Xrm.Page.getAttribute(attributeName);
}
function GetValueAttribute(attributeName) {
    if (Xrm.Page.getAttribute(attributeName))
        return Xrm.Page.getAttribute(attributeName).getValue();
}
function SetValueAttribute(attributeName, Value) {
    Xrm.Page.getAttribute(attributeName).setValue(Value);
}
function SetVisible(controlName, value) {
    Xrm.Page.getControl(controlName).setVisible(value)
}
function SetFocus(controlName) {
    Xrm.Page.getControl(controlName).setFocus(true);
}
function IsCreateForm() {
    if (Xrm.Page.ui.getFormType() == 1) {
        return true;
    }

    return false;
}
function IsEditForm() {
    if (Xrm.Page.ui.getFormType() == 2) {
        return true;
    }

    return false;
}
function bindTafkeet(cNumberName, cWordsName) {

    var controlNumber = document.getElementById(cNumberName);
    if (controlNumber == null) {
        alert(cNumberName + " is not Exist");
        return;
    }

    controlNumber.setAttribute('onchange', function () {
        var words = callAjax("https://" + window.location.hostname + ":8001/Tafkeet.ashx?Number=" + controlNumber.value);
        Xrm.Page.getAttribute(cWordsName).setValue(words);
    });

    if (GetValueAttribute(cNumberName) > 0 && GetValueAttribute(cWordsName) == null) {
        var words = callAjax("https://" + window.location.hostname + ":8001/Tafkeet.ashx?Number=" + controlNumber.value);
        Xrm.Page.getAttribute(cWordsName).setValue(words);
    }

}
function confirmYesNo(str) {
    execScript('n = msgbox("' + str + '","3")', "vbscript");
    return (n == 6);
}
function MessageBox(strMessage, title) {
    var strScript = 'n = msgbox("' + strMessage + '","3","' + title + '")';
    strScript = strScript.replace('nn', '"& chr(13) &"');
    execScript(strScript, "vbscript");
    if (n == 6) return 'yes';
    if (n == 7) return 'no';
    if (n == 2) return 'cancel';
    alert(n);
}

///******************************************************************************
function SaveAndCloseForm() {
    Xrm.Page.data.entity.save('saveandclose');
}
function SaveAndNew() {
    Xrm.Page.data.entity.save('saveandnew');
}
function openNewWindow(url) {
    var name = "newWindow";
    var width = 900;
    var height = 700;
    var newWindowFeatures = "status=1;scrollbars=1";

    // CRM function to open a new window
    return openStdWin(url, name, width, height, newWindowFeatures);

}

function openStdWin(url) {
    mywindow = window.open(url, "mywindow", "scrollbars=yes,resizable=yes,  width=900,height=700");
}
function OpenNewStandardWidow(url) {
    mywindow = window.open(url, "mywindow", "scrollbars=yes,resizable=yes,  width=800,height=600");

}
function bindLookUpToName(controlName, nameField) {

    var control = document.getElementById(controlName);
    var tempFunc =
        function () {
            var x = getLookUpDisplay(controlName);
            //alert(x);
            if (nameField == null) nameField = 'new_name';
            SetValueAttribute(nameField, x);
        };

    Xrm.Page.data.entity.attributes.get(controlName).addOnChange(tempFunc);
}
function BindOnChange(controlName, fnction) {

    Xrm.Page.data.entity.attributes.get(controlName).addOnChange(fnction);
}
function bindOnSave(fnction) {
    Xrm.Page.data.entity.addOnSave(fnction)
}
function bindSelectToName(controlName, nameField) {

    var control = document.getElementById(controlName);
    control.onchange =
        function () {
            var x = control[control.selectedIndex].text;
            if (nameField == null) nameField = 'new_name';
            SetValueAttribute(nameField, x);
        };

}

var LoadedArray = new Array();

function CalculateField(fieldName, equation) {
    var MainEquation = '';
    equation = equation.toLowerCase();
    MainEquation = equation;
    var tempEquation = equation.CustomReplace("\*", ',').CustomReplace('/', ',').CustomReplace('+', ',').CustomReplace('-', ',');
    tempEquation = tempEquation.CustomReplace('.', '').CustomReplace('0', '').CustomReplace('1', '').CustomReplace('2', '');
    tempEquation = tempEquation.CustomReplace('3', '').CustomReplace('4', '').CustomReplace('5', '').CustomReplace('6', '');
    tempEquation = tempEquation.CustomReplace('7', '').CustomReplace('8', '').CustomReplace('9', '');
    tempEquation = tempEquation.CustomReplace('(', '').CustomReplace(')', '');
    var controls = tempEquation.split(',');
    for (var i = 0; i < controls.length; i++) {
        if (controls[i] == '' || controls[i] == null) continue;
        var attribute = GetAttribute(controls[i]);
        if (LoadedArray[fieldName] == undefined) {
            var tempFunc = function () {
                CalculateField(fieldName, equation);
                FireOnChange(fieldName);
            };
            //attribute.addOnChange(tempFunc);
            BindOnChange(controls[i], tempFunc);
            // Xrm.Page.data.entity.attributes.get(controls[i]).addOnChange(tempFunc);

        }
        var value = GetValueAttribute(controls[i]);
        if (value == undefined) value = 0;
        MainEquation = MainEquation.replace(controls[i], value);


    }
    SetValueAttribute(fieldName, eval(MainEquation.replace('--', '+')));

    // ChangeFieldBackGround(fieldName, 'yellow');
    //SetEnabledState(fieldName, true);
    if (LoadedArray[fieldName] == undefined) {
        var tempDisableFunc = function () {
            SetEnabledState(fieldName, false);
        }
        Xrm.Page.data.entity.addOnSave(tempDisableFunc);
    }
    LoadedArray[fieldName] = 1;

}

function disableFormFields(onOff) {

    Xrm.Page.ui.controls.forEach(function (control, index) {
        if (doesControlHaveAttribute(control)) {
            if (control.getControlType() == 'lookup') {
                if (onOff == true) {
                    // DisableLookup(control.getName());
                }
            }
            control.setDisabled(onOff);
        }
    });
}
function removeMandatoryFormFields() {

    Xrm.Page.ui.controls.forEach(function (control, index) {
        if (doesControlHaveAttribute(control)) {
            setMandatory(control.getName(), false);

        }
    });
}

function doesControlHaveAttribute(control) {
    var controlType = control.getControlType();
    return controlType != "iframe" && controlType != "webresource" && controlType != "subgrid";
}

function disableSubgrid(subgridName) {
    var gridSpan = document.getElementById(subgridName + "_span");
    if (gridSpan.readyState != 'complete') {
        setTimeout("disableSubgrid('" + subgridName + "')", 1000);
    }

}
function calculateDetails(detailsEntityName, searchColumnName, fieldsComma) {

    var data = SelectEntityByRefrenceID(detailsEntityName, fieldsComma, searchColumnName, GetFormID());
    var fields = fieldsComma.split(',');
    var values = new Array();
    if (data.results.length > 0) {
        for (var i = 0; i < data.results.length; i++) {
            for (var j = 0; j < fields.length; j++) {
                if (values[fields[j]] == null) values[fields[j]] = 0;
                var attribute = eval('data.results[i].' + fields[j]);
                if (attribute != null && attribute.Value == null) {

                    values[fields[j]] = values[fields[j]] + Number(attribute);
                }
                if (attribute != null && attribute.Value != null) {

                    values[fields[j]] = values[fields[j]] + Number(attribute.Value);
                }
            }
        }
    }
    else {
        for (var j = 0; j < fields.length; j++) {
            values[fields[j]] = 0;
        }

    }
    return values;
}
function BindSubgridRefresh(gridName, gridEntity, filterByColumn, calculatedFields, totalFields) {

    /* CRM 2011
    //var grid = document.getElementById(gridName);
    var grid = Xrm.Page.getControl(gridName)._control.get_innerControl();
    //alert("ahmed");
    if (grid == null) {
    setTimeout("BindSubgridRefresh('" + gridName + "','" + gridEntity + "', '" + filterByColumn + "', '" + calculatedFields + "', '" + totalFields + "');", 1000);
    return;
    }

    // alert("ahmed");
    grid._events.addHandler('OnRefresh', function () { SetCalculatedFields(gridEntity, filterByColumn, calculatedFields, totalFields); });
    // grid.attachEvent("OnRefresh", function () { SetCalculatedFields(gridEntity, filterByColumn, calculatedFields, totalFields); });
    */


    //    function (){ SetCalculatedFields(gridEntity, filterByColumn, calculatedFields, totalFields); };

    SetCalculatedFields(gridEntity, filterByColumn, calculatedFields, totalFields);
    bindOnSave(function () { SetCalculatedFields(gridEntity, filterByColumn, calculatedFields, totalFields); })


}
function SetCalculatedFields(gridEntity, filterByColumn, calculatedFields, totalFields) {
    //alert("calc");
    if (IsEditForm()) {

        var arrValues = calculateDetails(gridEntity, filterByColumn, calculatedFields);
        var columns = totalFields.split(',');
        var fieldsValue = calculatedFields.split(',');
        for (var i = 0; i < columns.length; i++) {
            SetValueAttribute(columns[i], arrValues[fieldsValue[i]]);
            FireOnChange(columns[i]);
        }
    }

}
function FireOnChange(fielName) {
    Xrm.Page.getAttribute(fielName).fireOnChange();
}

function AddLabels(srcEntityNameDBCase, srcColumnsDBCase, SrcSearchFieldDBCase, frmSearchField, afterControlName, noOfColumns) {

    var GetColumnValue = function (entityColumn) {
        if (entityColumn == null)
            return "";

        if (entityColumn.Value != undefined) {
            return entityColumn.Value;
        }
        if (entityColumn.__metadata != null && entityColumn.__metadata.type == "Microsoft.Crm.Sdk.Data.Services.EntityReference") {
            if (entityColumn.Name != null) {
                return entityColumn.Name;
            }
            else return "";
        }

        if (entityColumn != null)
            return entityColumn;
    };

    var control = document.getElementById(afterControlName);

    var searchValue = getLookUpValue(frmSearchField);
    var parentTable = null;
    var InsertBeforeControl = null;
    if (control.tagName.toLowerCase() == "input") {
        parentTable = control.parentNode.parentNode.parentNode;
    }
    else {
        parentTable = control.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;
    }
    var Columns = srcColumnsDBCase.split(',');
    //var DisplayColumns = Labels.split(',');
    var entity = new Array();
    if (searchValue != null) {
        entity = GetEntityByGuid(srcEntityNameDBCase, SrcSearchFieldDBCase, getLookUpValue(frmSearchField)).results[0];
    }
    var tr = null;
    for (var i = 0; i < Columns.length; i++) {

        if (i % noOfColumns == 0) {
            tr = document.createElement('tr');
        }
        var td = document.createElement('td');
        td.setAttribute('classs', 'ms-crm-FieldLabel-LeftAlign ms-crm-Field-Normal');

        var label = document.createElement('label');
        // label.innerText = DisplayColumns[i];
        RetrieveAttributeLabel(srcEntityNameDBCase, Columns[i], label);
        td.appendChild(label);
        tr.appendChild(td);



        td = document.createElement('td');
        td.setAttribute('classs', 'ms-crm-FieldLabel-LeftAlign ms-crm-Field-Normal');
        td.setAttribute('nowrap', 'nowrap');
        label = document.createElement('span');

        if (searchValue != null) {
            var attribute = eval('entity.' + Columns[i]);
            var isOptionSet = false;

            if (attribute.__metadata != null && attribute.__metadata.type == "Microsoft.Crm.Sdk.Data.Services.OptionSetValue") {
                isOptionSet = true;
            }
            if (Columns[i].indexOf('status') < 0 && isOptionSet == false) {
                label.innerHTML = "<nobr class='ms-crm-Lookup-Item'>" + GetColumnValue(eval('entity.' + Columns[i])) + "</nobr>";
                label.setAttribute('title', GetColumnValue(eval('entity.' + Columns[i])));
                label.setAttribute('class', "ms-crm-Lookup-Item");
            }
            else {
                label.innerHTML = "<nobr class='ms-crm-Lookup-Item'>" + GetColumnValue(eval('entity.' + Columns[i])) + "</nobr>";
                label.setAttribute('title', GetColumnValue(eval('entity.' + Columns[i])));
                label.setAttribute('class', "ms-crm-Lookup-Item");
                RetrieveOptionsetLabel(srcEntityNameDBCase, Columns[i], attribute.Value, label);
            }
        }
        td.appendChild(label);
        tr.appendChild(td);

        if ((i + 1) % noOfColumns == 0) {
            parentTable.appendChild(tr);

        }
    }
    if (tr.parentNode == null) {
        parentTable(tr);
    }


}

//document.body.
function AddLabelsHorizontal(srcEntityNameDBCase, srcColumnsDBCase, SrcSearchFieldDBCase, frmSearchField, afterControlName, noOfColumns) {

    var GetColumnValue = function (entityColumn) {
        if (entityColumn == null)
            return "";

        if (entityColumn.Value != undefined) {
            return entityColumn.Value;
        }
        if (entityColumn.__metadata != null && entityColumn.__metadata.type == "Microsoft.Crm.Sdk.Data.Services.EntityReference") {
            if (entityColumn.Name != null) {
                return entityColumn.Name;
            }
            else return "";
        }

        if (entityColumn != null)
            return entityColumn;
    };

    var control = document.getElementById(afterControlName);

    var searchValue = getLookUpValue(frmSearchField);
    var parentTable = null;
    var InsertBeforeControl = null;
    var currentRow;
    if (control.tagName.toLowerCase() == "input") {
        parentTable = control.parentNode.parentNode.parentNode;
    }
    else {
        currentRow = control.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;
        parentTable = control.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;
    }
    currentRow.removeChild(currentRow.lastChild);
    //currentRow.removeChild(currentRow.lastChild);
    var Columns = srcColumnsDBCase.split(',');
    //var DisplayColumns = Labels.split(',');
    var entity = new Array();
    if (searchValue != null) {
        entity = GetEntityByGuid(srcEntityNameDBCase, SrcSearchFieldDBCase, getLookUpValue(frmSearchField)).results[0];
    }
    var tr = null;
    for (var i = 0; i < Columns.length; i++) {

        if (i % noOfColumns == 0) {
            tr = document.createElement('tr');
        }
        if (i > 0) {
            currentRow = tr;
        }
        var td = document.createElement('td');
        td.setAttribute('classs', 'ms-crm-FieldLabel-LeftAlign ms-crm-Field-Normal');

        var label = document.createElement('label');
        // label.innerText = DisplayColumns[i];
        RetrieveAttributeLabel(srcEntityNameDBCase, Columns[i], label);
        td.appendChild(label);
        currentRow.appendChild(td);



        td = document.createElement('td');
        td.setAttribute('classs', 'ms-crm-FieldLabel-LeftAlign ms-crm-Field-Normal');
        td.setAttribute('nowrap', 'nowrap');
        label = document.createElement('span');

        if (searchValue != null) {
            var attribute = eval('entity.' + Columns[i]);
            var isOptionSet = false;

            if (attribute.__metadata != null && attribute.__metadata.type == "Microsoft.Crm.Sdk.Data.Services.OptionSetValue") {
                isOptionSet = true;
            }
            if (Columns[i].indexOf('status') < 0 && isOptionSet == false) {
                label.innerHTML = "<nobr class='ms-crm-Lookup-Item'>" + GetColumnValue(eval('entity.' + Columns[i])) + "</nobr>";
                label.setAttribute('title', GetColumnValue(eval('entity.' + Columns[i])));
                label.setAttribute('class', "ms-crm-Lookup-Item");
            }
            else {
                label.innerHTML = "<nobr class='ms-crm-Lookup-Item'>" + GetColumnValue(eval('entity.' + Columns[i])) + "</nobr>";
                label.setAttribute('title', GetColumnValue(eval('entity.' + Columns[i])));
                label.setAttribute('class', "ms-crm-Lookup-Item");
                RetrieveOptionsetLabel(srcEntityNameDBCase, Columns[i], attribute.Value, label);
            }
        }
        td.appendChild(label);
        currentRow.appendChild(td);
        //  tr.appendChild(td);

        if ((i + 1) % noOfColumns == 0 && i > 0) {
            parentTable.appendChild(tr);

        }
    }
    if (tr.parentNode == null) {
        //parentTable(tr);
    }


}
function RetrieveAttributeLabel(entityName, attributeName, control) {
    // Entity schema name
    var entityLogicalName = entityName;
    // option set schema name
    var RetrieveAttributeName = attributeName;
    // Target Field schema name to which optionset text needs to be assigned
    var AssignAttributeName = control;

    // Option set value for which label needs to be retrieved

    var onSucessFn = function (logicalName, entityMetadata, RetrieveAttributeName, AssignAttributeName) {
        ///<summary>
        /// Retrieves attributes for the entity 
        ///</summary>

        var success = false;
        for (var i = 0; i < entityMetadata.Attributes.length; i++) {
            var AttributeMetadata = entityMetadata.Attributes[i];
            if (success) break;
            if (AttributeMetadata.SchemaName.toLowerCase() == RetrieveAttributeName.toLowerCase()) {
                control.innerText = AttributeMetadata.DisplayName.UserLocalizedLabel.Label;
                success = true;
                break;

            }

        }


    };

    // Calling Metadata service to get Optionset Label
    SDK.MetaData.RetrieveEntityAsync(SDK.MetaData.EntityFilters.Attributes, entityLogicalName, null, false, function (entityMetadata) { onSucessFn(entityLogicalName, entityMetadata, RetrieveAttributeName, AssignAttributeName); }, errorDisplay);

}

function RetrieveOptionsetLabel(entityName, attributeName, value, control) {
    // Entity schema name
    var entityLogicalName = entityName;
    // option set schema name
    var RetrieveAttributeName = attributeName;
    // Target Field schema name to which optionset text needs to be assigned
    var AssignAttributeName = control;

    // Option set value for which label needs to be retrieved
    var stateValue = value;


    // Calling Metadata service to get Optionset Label
    SDK.MetaData.RetrieveEntityAsync(SDK.MetaData.EntityFilters.Attributes, entityLogicalName, null, false, function (entityMetadata) { successRetrieveEntity(entityLogicalName, entityMetadata, RetrieveAttributeName, stateValue, AssignAttributeName); }, errorDisplay);

}

// Called upon successful metadata retrieval of the entity
function successRetrieveEntity(logicalName, entityMetadata, RetrieveAttributeName, OptionValue, AssignAttributeName) {
    ///<summary>
    /// Retrieves attributes for the entity 
    ///</summary>

    var success = false;
    for (var i = 0; i < entityMetadata.Attributes.length; i++) {
        var AttributeMetadata = entityMetadata.Attributes[i];
        if (success) break;
        if (AttributeMetadata.SchemaName.toLowerCase() == RetrieveAttributeName.toLowerCase()) {
            for (var o = 0; o < AttributeMetadata.OptionSet.Options.length; o++) {
                var option = AttributeMetadata.OptionSet.Options[o];
                if (option.OptionMetadata != null && option.OptionMetadata.Value == OptionValue) {
                    AssignAttributeName.innerText = option.OptionMetadata.Label.UserLocalizedLabel.Label;
                    success = true;
                    break;
                }
                if (option.StatusOptionMetadata != null && option.StatusOptionMetadata.Value == OptionValue) {
                    AssignAttributeName.innerText = option.StatusOptionMetadata.Label.UserLocalizedLabel.Label;
                    success = true;
                    break;
                }
            }
        }

    }


}

function errorDisplay(XmlHttpRequest, textStatus, errorThrown) {

    alert(errorThrown);
}

//************************************************************************************************************
//**************************************** By Ali Diab******************************************************
function CheckPhoneControl(cName) {
    var control = document.getElementById(cName);
    control.title = 'ملحوظة:رقم التليفون أو الفاكس يجب أن يبدأ بصفر  ';
    control.onchange = function () {
        var check = checkIsPhone(control.value);
        if (!check) {
            alert("من فضلك قم بإدخال رقم هاتف صحيح يبدا بـ 0 .مثال" + '\n' + "05xxxxxxxx - 02xxxxxxxx");
            control.value = '';
            control.focus();
        }
    }
}
function checkIsPhone(phonenumber) {
    var IsPhone = false;
    //var pattern = /^((00|\+)966)\d{8,9}$/;
    var pattern = /^((0))\d{8,10}$/;

    if (pattern.test(phonenumber)) {
        IsPhone = true;
    }
    else {
        IsPhone = false;
    }
    return IsPhone;
}

/*function CheckMobileControl(cName) {
    var control = document.getElementById(cName);
    control.title = 'ملحوظة:رقم الجوال يجب أن يبدأ بصفر  ';
    control.onchange = function () {
        var check = IsMobile(control.value);
        if (!check) {
            alert("من فضلك قم بإدخال رقم جوال صحيح يبدأ برقم 0 .مثال" + '\n' + "05xxxxxxxx");
            control.value = '';
            control.focus();
        }
    }
}*/
function IsMobile(MobileNumber) {
    var IsPhone = false;
    //var pattern = /^((00|\+)966)\d{8,9}$/;
    var pattern = /^((0))\d{9}$/;

    if (pattern.test(MobileNumber)) {
        IsPhone = true;
    }
    else {
        IsPhone = false;
    }
    return IsPhone;
}
/// التحقق من ان المدخلات أرقام فقط
function Fun_NumbersOnly(ControlId) {
    //var pattern = /[0-9]+/g;
    var pattern = new RegExp("^[-]?[0-9]+[\.]?[0-9]+$");
    var control = document.getElementById(ControlId);
    control.onchange = function () {
        if (!pattern.test(control.value)) {
            alert("رقم الهاتف لا يقبل سوى أرقام");
            control.value = '';
            control.focus();
        }
    }
}

///وضع  "/" أثناء كتابة التاريخ
function SetDateSeparator(FieldName) {
    var Ctrl = document.getElementById(FieldName);
    Ctrl.onkeydown = function (e) {
        e = e || window.event;
        var charCode = e.which || e.keyCode;
        if (charCode == 111 || charCode == 191) {
            return false;
        }
    }
    Ctrl.onkeyup = function (evt) {
        evt = evt || window.event;
        var charCode = evt.which || evt.keyCode;
        if (charCode != 8 && charCode != 17) {
            var len = Ctrl.value.length;
            if (len == 2 || len == 5) {
                Ctrl.value = Ctrl.value + "/";
            }
        }
    };
}

function SetDateSeperatorForCalender(controlName) {
    var parentControl = document.getElementById(controlName);
    var Ctrl = parentControl.getElementsByTagName("input")[0];

    Ctrl.onkeydown = function (e) {
        e = e || window.event;
        var charCode = e.which || e.keyCode;
        if (charCode == 111 || charCode == 191) {
            return false;
        }
    }
    Ctrl.onkeyup = function (evt) {
        evt = evt || window.event;
        var charCode = evt.which || evt.keyCode;
        if (charCode != 8 && charCode != 17) {
            var len = Ctrl.value.length;
            if (len == 2 || len == 5) {
                Ctrl.value = Ctrl.value + "/";
            }
        }
    };
}
///وضع  "/" أثناء كتابة التاريخ
function SetDateSeparator2() {
    var Ctrl = GetAttribute('DateInput');
    Ctrl.onkeydown = function (e) {
        alert('test');
    };
}

function SaveForm() {
    Xrm.Page.data.entity.save();

}

//function ChangeFieldBackGround(fieldname, color) {

//    //crmForm.all[fieldname].style.backgroundColor = color;
//    var control = document.getElementById(fieldname);
//    if (control == null)
//        return;

//    var controles = document.getElementById(fieldname).parentNode.parentNode.getElementsByTagName('input');
//    for (var i = 0; i < controles.length; i++) {
//        controles[i].style.backgroundColor = color;
//        controles[i].style.color = 'blue';
//        controles[i].style.fontWeight = 'bold';
//    }
//    controles = document.getElementById(fieldname).parentNode.parentNode.getElementsByTagName('select');
//    for (var i = 0; i < controles.length; i++) {
//        controles[i].style.backgroundColor = color;
//        controles[i].style.color = 'blue';
//        //   controles[i].style.fontWeight='bold';
//    }
//    //SetEnabledState(fieldname, false);
//}

function ChangeFieldBackGround2(fieldname, color) {
    crmForm.all[fieldname].style.backgroundColor = color;
}

function CheckDateFormat(control) {
    var minYear = 0001;
    var maxYear = (new Date()).getFullYear() - 575;
    var pattern = /^(\d{1,2})\/(\d{1,2})\/(\d{4})$/;
    var errorMsg = "";
    var field = document.getElementById(control);
    field.onchange = function () {
        if (field.value != '') {
            if (regs = field.value.match(pattern)) {
                if (regs[1] < 1 || regs[1] > 31) {
                    errorMsg = "خطأ قيمة اليوم هذه: " + regs[1];
                }
                else if (regs[2] < 1 || regs[2] > 12) {
                    errorMsg = "خطأقيمة الشهر هذه: " + regs[2];
                }
                else if (regs[3] < minYear) {
                    errorMsg = "خطأ قيمة السنة هذه: " + regs[3]; // + " - must be between " + minYear + " and " + maxYear;
                }
                else {
                    errorMsg = "";
                }
            }
            else {
                errorMsg = "هذا التاريخ خاطى يرجى كتابته على شكل :يوم/شهر/ سنة ";
            }
        }
        if (errorMsg != "") {
            alert(errorMsg);
            field.value = '';
            field.focus();
            return false;
        }
    }
}

///انشاء مربع نص التحويلة
function CreatPhoneExtension(control) {
    var field = document.getElementById(control);
    var PhoneID = field.id;
    if (document.getElementById(PhoneID) != null) {
        field.style.width = '65%'

        var labelTag = document.createElement("label");
        labelTag.id = "lbl" + PhoneID;
        labelTag.innerHTML = "تحويلة ";
        labelTag.style.width = '25px'

        var btnExtension = document.createElement('input');
        var Ext_Id = PhoneID + 'Ext';
        btnExtension.setAttribute('id', Ext_Id);
        btnExtension.setAttribute('type', 'text');
        btnExtension.title = 'التحويلة';
        btnExtension.style.width = '23%'
        field.parentNode.appendChild(labelTag);
        field.parentNode.appendChild(btnExtension);
        field.parentNode.setAttribute('nowrap', 'nowrap');
    }
}

//// Date Compare
var DatesCompare = function (a, b) {
    return (
        isFinite(a = this.Convertdate(a).valueOf()) &&
            isFinite(b = this.Convertdate(b).valueOf()) ?
            (a > b) - (a < b) :
            NaN
    );
}

function Convertdate(d) {
    return (
        d.constructor === Date ? d :
            d.constructor === Array ? new Date(d[0], d[1], d[2]) :
                d.constructor === Number ? new Date(d) :
                    d.constructor === String ? new Date(d) :
                        typeof d === "object" ? new Date(d.year, d.month, d.date) :
                            NaN
    );
}

////  مقارنة التاريخ المدخل مع تاريخ اليوم
function CompareDatewithToday(FieldName) {
    var FieldDate = GetValueAttribute(FieldName);
    var today = new Date();
    var result = DatesCompare(FieldDate.getDate(), today.getDate());
    if (result < 0) {
        alert("لا يمكن لهذا التاريخ أن يكون قبل تاريخ اليوم");
        SetValueAttribute(FieldName, null);
        SetFocus(FieldName);
    }
}
//// مقارنة تاريخ البداية وتاريخ النهاية
function CompareStartEndDates(StartDate, EndDate) {
    var start_date = GetValueAttribute(StartDate);
    var end_date = GetValueAttribute(EndDate);
    if (start_date != null && end_date != null) {
        //var result = DatesCompare(start_date.getDate(), end_date.getDate());
        //if (result >= 0) {
        //alert("لا يمكن لتاريخ الإصدارأن يساوى أو أكبر من تاريخ الإنتهاء");
        //SetValueAttribute(EndDate, null);
        //SetFocus(EndDate);
        //}
        if (Date.parse(start_date) > Date.parse(end_date)) {
            alert("لا يمكن لتاريخ الإصدارأن يساوى أو أكبر من تاريخ الإنتهاء");
            SetValueAttribute(EndDate, null);
            SetFocus(EndDate);
        }
    }
}

////حروف عربية فقط
function ArabicCharOnly(FieldName) {
    var Control = document.getElementById(FieldName);
    Control.title = 'أدخل اسماء بالعربية فقط';  //ToolTip
    var pattern = new RegExp("[a-zA-Z]+$");
    var reg = /^[-]?[0-9]+[\.]?[0-9]+$/;
    Control.onkeypress = function (evt) {
        evt = evt || window.event;
        var charCode = evt.which || evt.keyCode;
        var Ch = String.fromCharCode(charCode);
        if (pattern.test(Ch)) {
            alert("فضلا أدخل حروف عربية فقط");
            return false;
        }
    };
}

////حروف انجليزية فقط
function EnglishCharOnly(FieldName) {
    var Control = document.getElementById(FieldName);
    Control.title = 'فضلا أدخل حروف انجليزية فقط';  //ToolTip
    var pattern = new RegExp("[A-Za-z0-9 ._]");
    var reg = /^[-]?[0-9]+[\.]?[0-9]+$/;
    Control.onkeypress = function (evt) {
        evt = evt || window.event;
        var charCode = evt.which || evt.keyCode;
        var Ch = String.fromCharCode(charCode);
        if (!pattern.test(Ch)) {
            alert("فضلا أدخل حروف انجليزية فقط");
            return false;
        }
    };
}

///.......... Fill Lookup By Loggedin User ..............

function FillLookupByLoggedinUser(lookupId) {
    var curUserId = Xrm.Page.context.getUserId();
    var results = GetEntityByGuid('SystemUser', 'SystemUserId', curUserId)
    if (results != null) {
        var userName = results.results[0].FullName;
        SetLookupValue(lookupId, curUserId, userName, 'SystemUser');
    }
}
///.......... Fill Branch Lookup By Loggedin User ..............

function FillBranchLookup(lookupId) {
    var curUserId = Xrm.Page.context.getUserId();
    var results = GetEntityByGuid('SystemUser', 'SystemUserId', curUserId)
    if (results != null) {
        var BranchId = results.results[0].BusinessUnitId.Id;
        var BranchName = results.results[0].BusinessUnitId.Name;
        if (BranchId != null) {
            if (getLookUpValue(lookupId) == null) {
                SetLookupValue(lookupId, BranchId, BranchName, 'BusinessUnit');
            }
        }
    }
}
///..................................... Set Section Fields values to Null ..........................................

function SetSectionFieldsNull(sectionName) {
    Xrm.Page.ui.controls.forEach(function (control, index) {
        if (control.getParent().getName() == sectionName) {
            if (control.getName().indexOf('note') <= 0) {
                SetValueAttribute(control.getName(), null);
            }
        }
    });
}
///..................................... Disable LookupField Links ..........................................
function DisableLookup(FieldName) {
    var lookupParentNode = document.getElementById(FieldName + "_d");
    var lookupSpanNodes = lookupParentNode.getElementsByTagName("SPAN");

    for (var spanIndex = 0; spanIndex < lookupSpanNodes.length; spanIndex++) {
        var currentSpan = lookupSpanNodes[spanIndex];
        // Hide the hyperlink formatting
        currentSpan.style.textDecoration = "none";
        currentSpan.style.color = "#000000";
        // Revoke click functionality
        currentSpan.onclick = function () { };
    }
}
//************************************************************************************************************
//************************************************************************************************************
function EntityReference(id, name, entityLogicalName) {

    return { Id: id, LogicalName: entityLogicalName, Name: name };
}
function OptionSet(numberValue) {
    return { Value: Number(numberValue) }
}
function Money(strValue) {
    var Revenue = { Value: strValue };
    return Revenue;
}
function CreateRecord(entityNameDBCase, Record) {
    var ODataPath = getServerurl() + "/XRMServices/2011/OrganizationData.svc";

    var createNewAccount = new XMLHttpRequest();
    createNewAccount.open("POST", ODataPath + "/" + entityNameDBCase + "Set", false);
    createNewAccount.setRequestHeader("Accept", "application/json");
    createNewAccount.setRequestHeader("Content-Type", "application/json; charset=utf-8");

    createNewAccount.send(JSON.stringify(Record));
    var createAccountReturnValue = JSON.parse(createNewAccount.responseText).d;
    return createAccountReturnValue;

}

function UpdateRecordString(entityNameDbCase, id, fieldName, newValue) {
    var Record = {};
    eval('Record.' + fieldName + '="' + newValue + '";');
    UpdateRecord(entityNameDbCase, Record, id);
}
function UpdateRecord(entityNameDbCase, Record, id) {
    var ODataPath = getServerurl() + "/XRMServices/2011/OrganizationData.svc";
    //var ODataPath = "http;//crm1:5555/osoulmodern/XRMServices/2011/OrganizationData.svc";
    var req = new XMLHttpRequest();
    req.open("POST", ODataPath + "/" + entityNameDbCase + "Set(guid'" + id + "')", false);
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.setRequestHeader("X-HTTP-Method", "MERGE");
    req.send(JSON.stringify(Record));
    return req.responseText;

}

///********************** User Roles **************************************************************************************
function IsUserInRole(RoleId) {
    var UserRoles = Xrm.Page.context.getUserRoles();

    for (var i = 0; i < UserRoles.length; i++) {
        var Role = UserRoles[i];

        if (GuidsAreEqual(Role, RoleId)) {
            return true;
        }
    }
    return false;
}

function GuidsAreEqual(guid1, guid2) {
    var isEqual = false;

    if (guid1 == null || guid2 == null) {
        isEqual = false;
    }
    else {
        isEqual = guid1.replace(/[{}]/g, "").toLowerCase() == guid2.replace(/[{}]/g, "").toLowerCase();
    }
    return isEqual;
}
///********************** Current User ID **************************************************************************************
function CurrentUserID() {
    var CurUserId = Xrm.Page.context.getUserId();
    return CurUserId;
}

///..................................إنشاء دفعة.................................

function CreatNewPayment(AccountId, PaymentType, Amount, CreditNoteId, PaymentMethod) {

    var PaymentRecord = {};
    if (PaymentType != null) {
        PaymentRecord.new_PaymentType = { Value: Number(PaymentType) };
    }
    else {
        PaymentRecord.new_PaymentType = { Value: 1 };
    }
    if (PaymentMethod != null) {
        PaymentRecord.new_PaymentMethod = OptionSet(PaymentMethod);
    }
    else {
        PaymentRecord.new_PaymentMethod = OptionSet('100000000');
    }

    if (Amount != null) {
        PaymentRecord.new_Amount = { Value: Amount };
    }

    if (AccountId != null) {
        PaymentRecord.new_accountid = EntityReference(AccountId, '', 'account');
    }

    if (CreditNoteId != null) {
        PaymentRecord.new_CreditNoteId = EntityReference(CreditNoteId, '', 'new_creditnote');
    }

    //  PaymentRecord.new_PaymentMethod = { Value: 100000000 };
    ///............................................Create..............................................................
    var RetPayment = CreateRecord('new_installmentpayment', PaymentRecord);
    return RetPayment;
}

function ConvertStrToDate(fieldValue) {
    var strDate = fieldValue.substring(fieldValue.indexOf('(') + 1, fieldValue.indexOf(')'));
    var date = new Date(Number(strDate));
    return date;
}


function updateFetchXml(fetchxml, field, id) {
    var offset = 19;
    var fi = fetchxml.indexOf('<condition attribute="' + field + '" operator="eq" ');
    var nfetchxml = '';
    var closings = '';
    if (fi >= 0) {
        nfetchxml = fetchxml.substring(0, fi);
        offset = fetchxml.indexOf('/>', fi) - fi + 2;
    } else {
        fi = fetchxml.indexOf('<filter type="and">');
        if (fi < 0) {
            offset = 0;
            fi = fetchxml.indexOf('<entity');
            fi = fetchxml.indexOf('/>', fi) + 2;
            closings = '</filter>';
        }
        nfetchxml = fetchxml.substring(0, fi);
        nfetchxml += '<filter type="and">';
    }

    nfetchxml += '<condition attribute="' + field + '" ';
    nfetchxml += 'operator="eq" value="' + id + '" />';
    nfetchxml += closings;
    nfetchxml += fetchxml.substring(fi + offset);
    return nfetchxml;
}
function addFetchXmlcondition(fetchxml, condition) {
    var offset = 19;
    var fi = fetchxml.indexOf(condition);
    var nfetchxml = '';
    var closings = '';
    if (fi >= 0) {
        nfetchxml = fetchxml.substring(0, fi);
        offset = fetchxml.indexOf('/>', fi) - fi + 2;
    } else {
        fi = fetchxml.indexOf('<filter type="and">');
        if (fi < 0) {
            offset = 0;
            fi = fetchxml.indexOf('<entity');
            fi = fetchxml.indexOf('/>', fi) + 2;
            closings = '</filter>';
        }
        nfetchxml = fetchxml.substring(0, fi);
        nfetchxml += '<filter type="and">';
    }

    nfetchxml += condition;
    nfetchxml += closings;
    nfetchxml += fetchxml.substring(fi + offset);
    return nfetchxml;
}

function s4() {
    return Math.floor((1 + Math.random()) * 0x10000)
        .toString(16)
        .substring(1);
}

function guid() {
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
        s4() + '-' + s4() + s4() + s4();
}
function FilterLookUp(controlName, entity, curformFields, filterFieldsName) {
    var formFields = curformFields.split(',');
    var defaultViewId = Xrm.Page.getControl(controlName).getDefaultView();
    var viewEntity = GetEntityByGuid('SavedQuery', 'SavedQueryId', defaultViewId).results[0];
    var filterData = function () {
        //get defaul view id


        //Random Guid
        var viewId = '{1DFB2B35-B07C-44D1-868D-258DEEAB88E2}';
        var fetchXml = viewEntity.FetchXml;

        var filterFields = filterFieldsName.split(',');
        for (var i = 0; i < formFields.length; i++) {
            try {
                debugger;
                if (getLookUpValue(formFields[i]) != null) {
                    if (getLookUpValue(formFields[i]) != null && getLookUpValue(formFields[i]) != 'undefinded') {
                        fetchXml = updateFetchXml(fetchXml, filterFields[i], getLookUpValue(formFields[i]));
                    }
                }
            }
            catch (e) {
                if (GetValueAttribute(formFields[i]) != null) {
                    if (GetValueAttribute(formFields[i]) != null && GetValueAttribute(formFields[i]) != 'undefinded') {
                        fetchXml = updateFetchXml(fetchXml, filterFields[i], GetValueAttribute(formFields[i]));
                    }
                }

            }

        }
        //alert(fetchXml);

        var LayoutXml = viewEntity.LayoutXml;
        LayoutXml = LayoutXml.replace('icon="1"', 'icon="0"');
        LayoutXml = LayoutXml.replace('preview="1"', 'preview="0"');
        //var Entity = viewEntity.entity;
        var QueryId = viewId;
        var viewDisplayName = "Filter View";
        Xrm.Page.getControl(controlName).addCustomView(viewId, entity, viewDisplayName, fetchXml, LayoutXml, true);
    }
    for (var j = 0; j < formFields.length; j++) {
        BindOnChange(formFields[j], filterData);
    }
    filterData();
}

function FilterLookUpWithCondition(controlName, entity, curformFields, filterFieldsName, condition) {

    var defaultViewId = Xrm.Page.getControl(controlName).getDefaultView();
    var viewEntity = GetEntityByGuid('SavedQuery', 'SavedQueryId', defaultViewId).results[0];
    var formFields = null;
    var filterData = function () {
        //get defaul view id


        //Random Guid
        var viewId = '{1DFB2B35-B07C-44D1-868D-258DEEAB88E2}';
        var fetchXml = viewEntity.FetchXml;
        if (condition != null) {
            fetchXml = addFetchXmlcondition(fetchXml, condition);
        }
        if (filterFieldsName != null && filterFieldsName != "") {
            var filterFields = filterFieldsName.split(',');
            formFields = curformFields.split(',');
            for (var i = 0; i < formFields.length; i++) {
                /*
                if (Xrm.Page.getControl(formFields[i]) != 'lookup') {
                if (GetValueAttribute(formFields[i]) != null && GetValueAttribute(formFields[i]) != 'undefinded') {
                fetchXml = updateFetchXml(fetchXml, filterFields[i], GetValueAttribute(formFields[i]));
                }
                }
                else
                */
                {
                    if (getLookUpValue(formFields[i]) != null && getLookUpValue(formFields[i]) != 'undefinded') {
                        fetchXml = updateFetchXml(fetchXml, filterFields[i], getLookUpValue(formFields[i]));
                    }
                }
            }
        }
        //alert(fetchXml);

        var LayoutXml = viewEntity.LayoutXml;
        //var Entity = viewEntity.entity;
        var QueryId = viewId;
        var viewDisplayName = "Filter View";
        Xrm.Page.getControl(controlName).addCustomView(viewId, entity, viewDisplayName, fetchXml, LayoutXml, true);
    }
    if (formFields != null) {
        for (var j = 0; j < formFields.length; j++) {
            BindOnChange(formFields[j], filterData);
        }
    }
    filterData();
}

function SetDefault(fieldName, value) {
    if (GetValueAttribute(fieldName) == null || GetValueAttribute(fieldName) == 'undefined') {
        SetValueAttribute(fieldName, value);
    }
}
function SetDefaultOrZero(fieldName, value) {
    if (GetValueAttribute(fieldName) == null || GetValueAttribute(fieldName) == 'undefined' || GetValueAttribute(fieldName) == 0) {
        SetValueAttribute(fieldName, value);
    }
}
function OpenPopUpWindow(url) {
    var strWindowFeatures = "menubar=yes,location=yes,resizable=yes,scrollbars=yes,status=yes";
    windowObjectReference = window.open(url, "CNN_WindowName", strWindowFeatures);
}


function SetLookUpResult(formField, resultDBField, resultRow) {
    var fieldVal = eval('resultRow.' + resultDBField);
    if (fieldVal.Id != null) {
        SetLookupValue(formField, fieldVal.Id, fieldVal.Name, fieldVal.LogicalName);
    }
}


function BindCheckWithDate(checkFieldName, checkValue, dateFieldName, optionsetFieldName, optionValue, previousValue) {

    BindOnChange(checkFieldName, function () {
        if (GetValueAttribute(checkFieldName) == checkValue) {
            if (dateFieldName != '')
                SetValueAttribute(dateFieldName, new Date());
            SetValueAttribute(optionsetFieldName, optionValue);

        }
        else {
            if (dateFieldName != '')
                SetValueAttribute(dateFieldName, null);
            if (previousValue != null)
                SetValueAttribute(optionsetFieldName, previousValue);
            else
                SetValueAttribute(optionsetFieldName, 1);

        }
    });
}

function checkUniqueness(entityName, fieldName, displayName) {

    BindOnChange(fieldName.toString().toLowerCase(), function () {
        var results = GetEntityByString(entityName, fieldName, GetValueAttribute(fieldName.toString().toLowerCase())).results;
        if (IsCreateForm()) {
            if (results != null && results.length > 0) {
                alert(displayName + " is already exist. Please inserted a different value");
                SetValueAttribute(fieldName.toString().toLowerCase(), null);
            }
        }
        else if (results != null && results.length > 1) {
            alert(displayName + " is already exist. Please inserte a different value");
            SetValueAttribute(fieldName.toString().toLowerCase(), null);
        }
    });
}

function checkUniquenessAjax(tableName, fieldName, curAttributeName, displayName) {

    BindOnChange(curAttributeName.toString().toLowerCase(), function () {

        //  if (IsCreateForm()) 
        {
            var value = GetValueAttribute(curAttributeName.toString().toLowerCase());
            var url = getServerurl() + "/ISV/CheckValueExist.ashx?tablename=" + tableName + "&fieldname=" + fieldName + "&fieldvalue=" + value;
            var count = callAjax(url)

            if (count != null && Number(count) > 0) {
                SetValueAttribute(curAttributeName.toString().toLowerCase(), null);
                alert(" يوجد سجل آخر للحقل" + " " + displayName + " بنفس قيمة " + value);
                // return true;
            }


        }

    });
}
function checkUniquenessAjaxList(tableName, columns, fields, displayName) {

    //var value = GetValueAttribute(curAttributeName.toString().toLowerCase());
    var fieldArray = fields.split(',');
    var values = "";
    var value = null;
    for (i = 0; i < fieldArray.length; i++) {

        if (IsLookUp(fieldArray[i])) {
            value = getLookUpValue(fieldArray[i]);
        }
        else {

            value = GetValueAttribute(fieldArray[i]);
        }
        if (value == null)
            return;
        values += value;
        if (i < fieldArray.length - 1)
            values += ",";
    }
    var url = getServerurl() + "/ISV/CheckValueExist02.ashx?tablename=" + tableName + "&fieldname=" + columns + "&fieldvalue=" + values;
    var count = callAjax(url)

    if (count != null && Number(count) > 0) {
        for (var i = 0; i < fieldArray.length; i++) {
            SetValueAttribute(fieldArray[i], null);
            SetMandatory(fieldArray[i], true);
        }
        // disableFormFields(true);

        alert(" يوجد سجل آخر للحقول" + " " + displayName + " بنفس القيمة ");
        // return true;
    }
}
function ConvertDateToString(date) {
    //var date = new Date();
    var curr_date = date.getDate();
    curr_date = ((curr_date <= 9) ? "0" : "") + curr_date.toString();
    var curr_month = date.getMonth() + 1; //Months are zero based
    curr_month = ((curr_month <= 9) ? "0" : "") + curr_month.toString();
    var curr_year = date.getFullYear();
    return curr_date + '/' + curr_month + '/' + curr_year;
}
function bindHijriDate(hijriDateField, georgianDateField) {
    function OnChangeHijri() {

        var hijriDate = GetValueAttribute(hijriDateField);
        var results = GetEntityByString("new_thijridate", "new_hijridateText", hijriDate).results;
        if (results != null && results.length > 0) {
            //  var fieldValue = eval(results[0].new_georgiandate);
            var dateValue = ConvertStrToDate(results[0].new_georgiandate);
            SetValueAttribute(georgianDateField, dateValue);
        }

    }
    function OnChangeGeorgian() {

        var georgianDate = GetValueAttribute(georgianDateField);
        var geostring = ConvertDateToString(georgianDate)
        var results = GetEntityByString("new_thijridate", "new_georgiantxt", geostring).results;
        if (results != null && results.length > 0) {
            SetValueAttribute(hijriDateField, results[0].new_hijridateText);
        }
    }

    BindOnChange(hijriDateField, OnChangeHijri);
    BindOnChange(georgianDateField, OnChangeGeorgian);

}
function GetNextSequence(prefixSeq) {
    //var url = getServerurl() + "/ISV/GETSequence.ashx";
    //url += "?seq=" + prefixSeq;

    //var result = callAjax(url);
    //return result;
    return null;
}
function OpenRecord(entityName, recordId) {
    window.open("/main.aspx?etn=" + entityName + "&pagetype=entityrecord&id=%7B" + recordId + "%7D", recordId);
}
function checkAndOpen(formFieldDbCase, entityNameDbCase, targetField) {

    BindOnChange(formFieldDbCase.toString().toLowerCase(), function () {
        var value = GetValueAttribute(formFieldDbCase.toString().toLowerCase());
        // var url = getServerurl() + "/ISV/ReturnGuidExist.ashx?tablename=" + entityName + "&fieldname=" + formField + "&fieldvalue=" + value;
        // entityNameDbCase + "Id",
        var results = GetEntityByString(entityNameDbCase, formFieldDbCase, value).results;
        if (results != null && results.length > 0) {
            var guid = results[0].entityNameDbCase + "Id";
            if (guid != "" && guid != null) {
                OpenRecord(entityNameDbCase.toLowerCase(), guid);
            }
        }
    });
}
function removeMandatoryForRole(roleID) {
    var userInRole = IsUserInRole(roleID);
    if (userInRole) {
        //remove all
        removeMandatoryFormFields(true);
    }

}

//Validate IQama and ID number
function CheckIdentity(IDNO) {
    var Result; // int
    var Type; //varchar(1);
    var i; //int;
    var ZFOdd; //varchar(max);
    var SubString; // varchar(1);
    var InTVALUE1; //int;
    var InTVALUE2; //int;
    var sum; //int;
    Result = 1;
    sum = 0;

    Type = IDNO.substr(0, 1);

    if (IDNO.length != 10) {
        return Result = 11;
    } else {

        if (Type != '1' && Type != '2') {
            return Result = 12;
        } else {
            var i = 0;
            while (i < 10) {
                if (i % 2 == 0) {
                    SubString = IDNO.substr(i, 1);
                    InTVALUE1 = parseInt(SubString) * 2;
                    if (InTVALUE1.toString().length == 1) {
                        ZFOdd = '0' + InTVALUE1.toString();
                    } else if (InTVALUE1.toString().length == 0) {
                        ZFOdd = '00';
                    } else {
                        ZFOdd = InTVALUE1.toString();
                    }
                    InTVALUE1 = parseInt(ZFOdd.substr(0, 1));
                    InTVALUE2 = parseInt(ZFOdd.substr(1, 1));
                    sum = sum + InTVALUE1 + InTVALUE2;

                } else {
                    sum = sum + parseInt(IDNO.substr(i, 1));

                }
                i = i + 1;
            } // while loop


        }
        Result = sum % 10;

    }
    return Result;
}

function checkId(idField) {
    var idnumber = GetValueAttribute(idField);
    if (CheckIdentity(idnumber) != 0) {
        alert("رقم الهوية غير صحيح");
        SetValueAttribute(idField, null);
    }

}

function checkIqamaNumber(idField) {

    BindOnChange(idField, function () { checkId(idField); });
}

function IsLookUp(controlName) {
    var type = Xrm.Page.getControl(controlName).getControlType();
    if (type == 'lookup') {
        return true;
    }
    else
        return false;
}
function IsNotLookUp(controlName) {
    var type = Xrm.Page.getControl(controlName).getControlType();
    if (type == 'lookup') {
        return false;
    }
    else
        return true;
}

function GetCurrentEntityName() {

    return Xrm.Page.data.entity.getEntityName();
}

function CheckStatusSecurity(fieldName, entityName, StatusValue, UserID) {

    var url = getServerurl() + "/ISV/ISVMawarid/GetTeamAndRole.ashx?userid=" + UserID + "&value=" + StatusValue + "&fieldName=" + fieldName + "&entityName=" + entityName;
    var Result = callAjax(url);

    return Result;

}

function OpenRecord(entityName, recordID) {
    Xrm.Utility.openEntityForm(entityName, recordID);
}

function OpenNewRecord(entityName) {
    Xrm.Utility.openEntityForm(entityName)
}

function OpenNewRecordParamters(entityName, fieldsComma, valuesComma) {

    var parameters = {};
    var fieldArray = fieldsComma.split(",");
    var valuesArray = valuesComma.split(",");
    if (fieldArray.length == valuesArray.length) {
        for (var i = 0; i < fieldArray.length; i++) {
            parameters[fieldArray[i]] = valuesArray[i];
        }


        //pop incident form with default values
        Xrm.Utility.openEntityForm(entityName, null, parameters);
    }
    else {
        alert("length is not same");
    }
}

//function ValidateEmail(email) {
//var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
//    if (re.test(email)) {
//return true;
// } else {
//return false;
//    }
//}

function setMaxValue(fieldName, value) {
    var control = document.getElementById(fieldName);
    if (control != null) {
        control.setAttribute("max", value);
    }
}

function setMinValue(fieldName, value) {
    var control = document.getElementById(fieldName);
    if (control != null) {
        control.setAttribute("min", value);
    }
}
/*function CallISVResponseURL(callURL) {
    var url = getServerurl() + "/ISV/GetUrlResponse.ashx?Link=" + callURL;

    var result = callAjax(url);
    return result;
}*/


function CheckNumberLength(fieldName, length) {

    var cid = GetValueAttribute(fieldName);
    var pattern = /[^0-9]/;

    if (pattern.test(cid)) {
        return false;
    } else if (cid.length < length || cid.length > length) {
        return false;
    }

    return true;

}

function GetIPAddress() {
    var url = getServerurl() + "/ISV/GetIPAddress.ashx";
    var result = callAjax(url);
    return result;
}

function preventAutoSave(econtext) {

    var eventArgs = econtext.getEventArgs();

    if (eventArgs.getSaveMode() == 70) {

        eventArgs.preventDefault();

    }

}


function addLookUpFilter(lookupName, fetchXMLFilter) {
    // add the event handler for PreSearch Event
    Xrm.Page.getControl(lookupName).addPreSearch(function () {

        Xrm.Page.getControl(lookupName).addCustomFilter(fetchXMLFilter);
    }

    );

}


function SetIframeSrc(frameName, url) {

    Xrm.Page.ui.controls.get(frameName).setSrc(url);
}


function GetCurrentUrlParamters() {

    var QueryString = "?id=" + GetFormID() + "&orglcid=1025&orgname=" + Xrm.Page.context.getOrgUniqueName() + "&typename=" + GetCurrentEntityName() + "&UID=" + CurrentUserID();

    return QueryString;
}

function OpenISVWindow(pagelocation) {
    var url = getISVWebUrl();
    url += pagelocation;
    url += GetCurrentUrlParamters();

    openNewWindow(url);

}


function OpenISVFromGrid(pagelocation) {
    var url = getISVWebUrl();
    url += pagelocation;
    // url += GetCurrentUrlParamters();

    openNewWindow(url);

}

function OpenNewlyMuqeem() {
    OpenISVFromGrid("Emp/Muqueem.aspx");
}
function BindDaysWithDates(daysControlName, startControlName, endControlName) {
    var startDate = GetValueAttribute(startControlName);
    var endDate = GetValueAttribute(endControlName);
    if (startDate != null && endDate != null) {


        if (GetValueAttribute(daysControlName) == null) {
            SetValueAttribute(daysControlName, GetDaysBetDate(startDate, endDate));

        }
    }
    BindOnChange(startControlName, function () {

        SetValueAttribute(daysControlName, GetDaysBetDate(GetValueAttribute(startControlName), GetValueAttribute(endControlName)));

    });
    BindOnChange(endControlName, function () {

        SetValueAttribute(daysControlName, GetDaysBetDate(GetValueAttribute(startControlName), GetValueAttribute(endControlName)));


    });

}

function GetDaysBetDate(startDate, endDate) {
    if (startDate && endDate) {
        var one_day = 1000 * 60 * 60 * 24;

        // Convert both dates to milliseconds
        var date1_ms = startDate.getTime();
        var date2_ms = endDate.getTime();

        // Calculate the difference in milliseconds
        var difference_ms = date2_ms - date1_ms;

        // Convert back to days and return
        return Math.round(difference_ms / one_day) + 1;
    }
}

function GetSignature(str) {
    var sum = 0;
    for (var i = 0; i < str.length; i++) {
        sum += str.charCodeAt(i);
    }
    sum = sum * sum;
    return sum.toString();

}

function SelectSQLDT(SQL) {


    var url = getISVWebUrl();
    url += "Settings/RunSQL.aspx"; // + GetCurrentUrlParamters();
    url += "?SQL=" + encodeURI(SQL);

    var strResponse = CallISVResponseURL(url);
    var dt = eval(strResponse);
    return dt;
}
function AddDaysToDate(date, days) {
    var someDate = date;
    var numberOfDaysToAdd = days * 24 * 60 * 60 * 1000;
    return new Date(someDate.getTime() + numberOfDaysToAdd);
}
function IsAdministrator() {
    //فريق الإدارة العليا
    if (checkCurrentUserInTeam("E7F1D5F2-88C0-E311-88BE-00155D010308")) {
        return true;
    }
    //system admin
    if (IsUserInRole("B4E4A039-F1AB-E611-80DD-00155D010C16")) {
        return true;
    }
    else
        return false;
}
function GetAllEntities() {
    var entityMetadataCollection = SDK.Metadata.RetrieveAllEntities(SDK.Metadata.EntityFilters.Entity, false);
    entityMetadataCollection.sort(function (a, b) {
        if (a.DisplayName.UserLocalizedLabel && b.DisplayName.UserLocalizedLabel) {
            if (a.DisplayName.UserLocalizedLabel.Label < b.DisplayName.UserLocalizedLabel.Label) { return -1 }
            if (a.DisplayName.UserLocalizedLabel.Label > b.DisplayName.UserLocalizedLabel.Label) { return 1 }
            return 0;
        }
        else if (!a.DisplayName.UserLocalizedLabel && !b.DisplayName.UserLocalizedLabel) {
            if (a.LogicalName < b.LogicalName) { return -1 }
            if (a.LogicalName > b.LogicalName) { return 1 }
            return 0;
        }
        else if (a.DisplayName.UserLocalizedLabel)
            return -1;
        else if (b.DisplayName.UserLocalizedLabel)
            return 1
    });
    return entityMetadataCollection;
}
function GetEntityMetaData(EntityLogicalName) {
    var entityMetadata = SDK.Metadata.RetrieveEntity(SDK.Metadata.EntityFilters.Attributes, EntityLogicalName, null, false);
    entityMetadata.Attributes.sort(function (a, b) {
        if (a.DisplayName.UserLocalizedLabel && b.DisplayName.UserLocalizedLabel) {
            if (a.DisplayName.UserLocalizedLabel.Label < b.DisplayName.UserLocalizedLabel.Label) { return -1 }
            if (a.DisplayName.UserLocalizedLabel.Label > b.DisplayName.UserLocalizedLabel.Label) { return 1 }
            return 0;
        }
        else if (!a.DisplayName.UserLocalizedLabel && !b.DisplayName.UserLocalizedLabel) {
            if (a.LogicalName < b.LogicalName) { return -1 }
            if (a.LogicalName > b.LogicalName) { return 1 }
            return 0;
        }
        else if (a.DisplayName.UserLocalizedLabel)
            return -1;
        else if (b.DisplayName.UserLocalizedLabel)
            return 1
    });
    return entityMetadata;
}
function GetAttributeMetaData(EntityLogicalName, AttributeLogicalName) {
    var AttributeMetadata = SDK.Metadata.RetrieveAttribute(EntityLogicalName, AttributeLogicalName, null, false);
    return AttributeMetadata;
}
function OpenSystemDoc() {
    OpenISVWindow("GenerateDocument.aspx");
}
var oldValues = [];
function CheckFieldSecurity(fieldName, entityName) {
    var matchUserFlag = false;
    var ValsFlag = false;
    var fetchXML = '<fetch mapping="logical" count="1" >' +
        '<entity name="new_fieldlevelsecurity" >' +
        '<filter type="and" >' +
        '<condition attribute="new_entityname" operator="eq" value="' + entityName + '" />' +
        '<condition attribute="new_fieldname" operator="eq" value="' + fieldName + '" />' +
        '</filter>' +
        '<link-entity name="new_fieldlevelsecuritydetails" from="new_fieldlevelsecurityid" to="new_fieldlevelsecurityid" >' +
        '<attribute name="new_fieldlevelsecuritydetailsid" alias="new_fieldlevelsecuritydetailsid" />' +
        '<attribute name="new_securitystatus" alias="new_securitystatus" />' +
        '<attribute name="new_type" alias="new_type" />' +
        '<attribute name="new_values" alias="new_values" />' +
        '</link-entity>' +
        '</entity>' +
        '</fetch>';
    var results = XrmServiceToolkit.Soap.Fetch(fetchXML);
    for (var i = 0; i < results.length; i++) {
        var status = results[i].attributes.new_securitystatus.value;
        var type = results[i].attributes.new_type.value;
        //status=1 hidden , 2 disabled ,3 restrictedValues
        if (status == 3) {
            oldValues[fieldName] = GetValueAttribute(fieldName);
            BindOnChange(fieldName,
                function () {
                    CheckStatusSecurityVals(fieldName, GetCurrentEntityName());
                }
            );
            continue;
        }
        var Users = GetEntityByGuid("new_new_fieldlevelsecuritydetails_systemuse", "new_fieldlevelsecuritydetailsid", results[i].attributes.new_fieldlevelsecuritydetailsid.value).results;
        var Teams = GetEntityByGuid("new_new_fieldlevelsecuritydetails_team", "new_fieldlevelsecuritydetailsid", results[i].attributes.new_fieldlevelsecuritydetailsid.value).results;
        for (var j = 0; j < Users.length; j++) {
            if (CurrentUserID().toLowerCase() == "{" + Users[j].systemuserid.toLowerCase() + "}") {
                matchUserFlag = true;
                break;
            }
        }
        for (var y = 0; y < Teams.length; y++) {
            if (checkCurrentUserInTeam(Teams[y].teamid)) {
                matchUserFlag = true;
                break;
            }
        }
        //type = false excluded , true included
        if ((matchUserFlag == true && type == false) || (matchUserFlag == false && type == true)) {
            if (status == 1) //hidden
                SetVisible(fieldName, true);
            else if (status == 2)//disabled
                setEnabled(fieldName);
            return;
        }
    }
    if (status == 1) //hidden
        SetVisible(fieldName, false);
    else if (status == 2)//disabled
        SetDisabledWithSaving(fieldName);
}
function CheckStatusSecurityVals(fieldName, entityName) {
    var matchUserFlag = false;
    var ValsFlag = false;
    var fetchXML = '<fetch mapping="logical" count="1" >' +
        '<entity name="new_fieldlevelsecurity" >' +
        '<filter type="and" >' +
        '<condition attribute="new_entityname" operator="eq" value="' + entityName + '" />' +
        '<condition attribute="new_fieldname" operator="eq" value="' + fieldName + '" />' +
        '</filter>' +
        '<link-entity name="new_fieldlevelsecuritydetails" from="new_fieldlevelsecurityid" to="new_fieldlevelsecurityid">' +
        '<attribute name="new_fieldlevelsecuritydetailsid" alias="new_fieldlevelsecuritydetailsid" />' +
        '<attribute name="new_securitystatus" alias="new_securitystatus" />' +
        '<attribute name="new_type" alias="new_type" />' +
        '<attribute name="new_values" alias="new_values" />' +
        '<filter type="and" >' +
        '<condition attribute="new_securitystatus" operator="eq" value="3" />' +
        '</filter>' +
        ' </link-entity>' +
        '</entity>' +
        "</fetch>";
    var results = XrmServiceToolkit.Soap.Fetch(fetchXML);
    for (var i = 0; i < results.length; i++) {
        //type = false excluded , true included
        var type = results[i].attributes.new_type.value;
        var values = results[i].attributes.new_values.value;
        var vals = values.split(',');
        var Users = GetEntityByGuid("new_new_fieldlevelsecuritydetails_systemuse", "new_fieldlevelsecuritydetailsid", results[i].attributes.new_fieldlevelsecuritydetailsid.value).results;
        var Teams = GetEntityByGuid("new_new_fieldlevelsecuritydetails_team", "new_fieldlevelsecuritydetailsid", results[i].attributes.new_fieldlevelsecuritydetailsid.value).results;
        for (var j = 0; j < Users.length; j++) {
            if (CurrentUserID().toLowerCase() == "{" + Users[j].systemuserid.toLowerCase() + "}") {
                matchUserFlag = true;
                break;
            }
        }
        if (matchUserFlag == false) {
            for (var y = 0; y < Teams.length; y++) {
                if (checkCurrentUserInTeam(Teams[y].teamid)) {
                    matchUserFlag = true;
                    break;
                }
            }
        }
        for (var x = 0; x < vals.length; x++) {
            if (GetValueAttribute(fieldName) == vals[x]) {
                ValsFlag = true;
                break;
            }
        }
        if ((matchUserFlag == true && ValsFlag == true && type == false) || (matchUserFlag == false && ValsFlag == true && type == true)) {
            return true;
        }
    }
    SetValueAttribute(fieldName, oldValues[fieldName]);
    alert("you don't have a permission to change");
    return false;
}
function BindStatusSecurity() {
    //Get All Security fields for current entity 
    var results = GetEntityByString("new_fieldlevelsecurity", "new_entityname", GetCurrentEntityName()).results;
    for (var i = 0; i < results.length; i++) {
        CheckFieldSecurity(results[i].new_fieldname, GetCurrentEntityName());
    }
}

function OpenISVWindowByParams(pagelocation, queryParams) {
    var url = getISVWebUrl();
    url += pagelocation;
    url += GetCurrentUrlParamters();
    url += "&Time=" + new Date().getTime();
    url += "&Sign=" + GetSignature(url);
    url += ("&" + queryParams);
    openNewWindow(url);
}

function GetCurrentUrlParamtersFromGrid(SelectedItems) {
    var queryString = "?id=" + SelectedItems[0].Id + "&orglcid=1025&orgname=" + Xrm.Page.context.getOrgUniqueName() + "&typename=" + GetCurrentEntityName() + "&UID=" + CurrentUserID();
    return queryString;
}

function OpenISVWindowFromGrid(pagelocation, SelectedItems) {
    debugger;
    var url = getISVWebUrl();
    url += pagelocation;
    url += GetCurrentUrlParamtersFromGrid(SelectedItems);
    url += "&Time=" + new Date().getTime();
    url += "&Sign=" + GetSignature(url);
    openNewWindow(url);

}



function GetVatRate(customerid, vatgroupfield) {
    debugger;
    results = GetEntityByGuid("Contact", "ContactId", customerid).results;
    if (results == null && results.length <= 0)
        var results = GetEntityByGuid("Account", "AccountId", customerid).results;
    if (results == null && results.length <= 0)
        results = GetEntityByGuid("new_agent", "new_agentId", customerid).results;


    var Rate = 0.05;
    if (results != null && results.length > 0) {

        var VatGroupId = results[0].new_vatgroupid.Id;
        if (VatGroupId == null)
            return Rate;

        var Vatresults = GetEntityByGuid("new_vatgroup", "new_vatgroupId", VatGroupId).results;
        console.log('Vatresults');
        console.log(Vatresults);
        if (vatgroupfield != 'undefined') {
            if (Vatresults[0].new_vatgroupId != null) {
                SetLookupValue(vatgroupfield, Vatresults[0].new_vatgroupId, Vatresults[0].new_name, "new_vatgroup");
            }

        }

        return Vatresults[0].new_rate;
    }

    return Rate;
}

function CalculateVateAmount(custFormFieldId, amountFormField, vatAmountField, vatRateField, vatgroupField) {
    debugger;
    var temVatCalc = function () {
        debugger;
        if (amountFormField == null) {
            amountFormField = "new_amount";
        }
        if (vatAmountField == null) {
            vatAmountField = "new_vatamount";
        }
        if (getLookUpValue(custFormFieldId) != null) {
            if (GetValueAttribute(amountFormField) != null) {
                var Rate = GetVatRate(getLookUpValue(custFormFieldId), vatgroupField);
                var VateAmount = GetValueAttribute(amountFormField) * Rate;
                if (vatRateField != null) {
                    SetValueAttribute(vatRateField, Rate);
                }
                SetValueAttribute(vatAmountField, VateAmount);
            }
        }
    }
    if (IsCreateForm()) {
        temVatCalc();
    }

    BindOnChange(custFormFieldId, temVatCalc);
    BindOnChange(amountFormField, temVatCalc);
}

function CalculateVateAmount(custFormFieldId, amountFormField, vatAmountField, vatRateField, vatgroupField, amountWithVat) {
    debugger;
    var temVatCalc = function () {
        debugger;
        if (amountFormField == null) {
            amountFormField = "new_amount";
        }
        if (vatAmountField == null) {
            vatAmountField = "new_vatamount";
        }
        if (getLookUpValue(custFormFieldId) != null) {
            if (GetValueAttribute(amountFormField) != null) {
                var Rate = GetVatRate(getLookUpValue(custFormFieldId), vatgroupField);
                var VateAmount = GetValueAttribute(amountFormField) * Rate;
                if (vatRateField != null) {
                    SetValueAttribute(vatRateField, Rate);
                }
                SetValueAttribute(vatAmountField, VateAmount);

                if (amountWithVat != 'undefined') {

                    SetValueAttribute(amountWithVat, VateAmount + GetValueAttribute(amountFormField));

                }
            }
        }
    }
    if (IsCreateForm()) {
        temVatCalc();
    }

    BindOnChange(custFormFieldId, temVatCalc);
    BindOnChange(amountFormField, temVatCalc);

}
function isNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}
function SetFormError(msg) {
    Xrm.Page.ui.setFormNotification(msg, "ERROR")
}
function SetFormINFORMATION(msg) {
    Xrm.Page.ui.setFormNotification(msg, "INFORMATION")
}
function SetFormWARNING(msg) {
    Xrm.Page.ui.setFormNotification(msg, "WARNING")
}
function clearFormNotification() {

    Xrm.Page.ui.clearFormNotification();
}
function addOption(controlname, opt) {
    var picklistControl = Xrm.Page.getControl(controlname);
    picklistControl.addOption(opt1);
}
function removeOption(controlname, optionValue) {
    var picklistControl = Xrm.Page.getControl(controlname);
    picklistControl.removeOption(optionValue);
}

function CalculateVatRateRelatedToVatGroup(amount, new_vatgroupid, new_vaterate, new_vatamount, new_amountwithvat) {
    var Vatresults = GetEntityByGuid("new_vatgroup", "new_vatgroupId", new_vatgroupid).results;
    console.log('Vatresults');
    console.log(Vatresults);
    if (Vatresults != null && Vatresults.length > 0) {
        var Rate = Vatresults[0].new_rate;
        SetValueAttribute(new_vaterate, Rate);
        var amountValue = GetValueAttribute(amount);
        var VateAmount = amountValue * Rate;

        SetValueAttribute(new_vatamount, VateAmount);
        SetValueAttribute(new_amountwithvat, VateAmount + amountValue);
    }
}
function GetDateDifferenceDays(from, to) {
    var from = GetValueAttribute(from);
    var to = GetValueAttribute(to);
    var fromDate = new Date(from.getFullYear(), from.getMonth(), from.getDate());
    var toDate = new Date(to.getFullYear(), to.getMonth(), to.getDate());
    var DayValue = 1000 * 60 * 60 * 24;
    var Days = Math.ceil((toDate - fromDate) / DayValue);
    return Days;
}
