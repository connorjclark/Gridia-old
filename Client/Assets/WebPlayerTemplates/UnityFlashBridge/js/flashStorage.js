var bridgeSwf;
var bridgeUnity;

function setUnityObject(unityObject) {
	unityBridge = unityObject;
}

function sendMessageToUnity(method, params) {
  unityBridge.SendMessage("FlashGameObject", method, params)
}

function unityBridgeInit(sharedObjectName, guid) {
  if (bridgeSwf == null) {
    bridgeSwf = document.getElementById("unity_flash_bridge");
  }
  result = bridgeSwf.sharedObjectInit(sharedObjectName);
  sendMessageToUnity("StringCallback", guid + "|" + result);
}

function unityBridgeGet(key, guid) {
  result = bridgeSwf.sharedObjectGet(key);
  sendMessageToUnity("StringCallback", guid + "|" + result);
}

function unityBridgePut(key, value, guid) {
  result = bridgeSwf.sharedObjectSet(key, value);
  sendMessageToUnity("StringCallback", guid + "|" + result);
}

function unityBridgeExists(key, guid) {
  result = bridgeSwf.sharedObjectGet(key) ? "success" : "failure";
  sendMessageToUnity("BooleanCallback", guid + "|" + result);
}

function unityBridgeDelete(key) {
  bridgeSwf.sharedObjectDelete(key);
}

function unityBridgeGetFiles(directory, option, guid) {
  result = bridgeSwf.sharedObjectGetFiles(directory, option);
  sendMessageToUnity("StringListCallback", guid + "|" + result);
}

function unityBridgeGetFilesRecursively(directory, option, guid) {
  result = bridgeSwf.sharedObjectGetFilesRecursively(directory, option);
  sendMessageToUnity("StringListCallback", guid + "|" + result);
}

function unityBridgeRequestUserToAdjustSharedObjectSettings(guid) {
  $("#unityPlayer").insertAfter("#flashDiv");
  bridgeSwf.sharedObjectRequestUserToAdjustSharedObjectSettings(guid);
}

function unityBridgeRequestUserToAdjustSharedObjectSettingsDone(guid) {
  $("#flashDiv").insertAfter("#unityPlayer");
  sendMessageToUnity("NoArgumentCallback", guid);
}

function unityBridgeRequestMinimumSize(size, guid) {
  result = bridgeSwf.sharedObjectRequestMinimumSize(size);
  sendMessageToUnity("StringCallback", guid + "|" + result);
}
