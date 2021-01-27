mergeInto(LibraryManager.library, {

 	AWSUploadToS3: function (logFileName, logFileText) 
	{
		var bucketName = "equal-i";
		var bucketRegion = "us-east-2";
		var IdentityPoolId = "us-east-2:37bb2809-f5de-4f36-96ef-51b4a576d1ab";

		AWS.config.update({
			region: bucketRegion,
			credentials: new AWS.CognitoIdentityCredentials({
				IdentityPoolId: IdentityPoolId
			})
		});

		var s3 = new AWS.S3({
			apiVersion: "2006-03-01",
			params: { Bucket: bucketName }
		});
		
		//window.alert("logFileName: " + Pointer_stringify(logFileName) + "  logFileText: " + Pointer_stringify(logFileText));

		// Use S3 ManagedUpload class as it supports multipart uploads
		var upload = new AWS.S3.ManagedUpload({
			params: {
				Bucket: bucketName,
				Key: Pointer_stringify(logFileName),
				Body: Pointer_stringify(logFileText),
				ACL: "private"
			}
		});

		var promise = upload.promise();

		promise.then(
			function(data) {
				//alert("Successfully uploaded log data.");
				var response = "Successfully uploaded log data.";
				unityInstance.SendMessage("AWSManager", "ServerResponse", response);
			},
			function(err) {
				var response = "There was an error uploading your log data: " + err.message;
				unityInstance.SendMessage("AWSManager", "ServerResponse", response);				
			}
		);

		//window.alert("Hello from Amazon");
	},
	
	Msg: function(msg)
	{
		window.alert(Pointer_stringify(msg));
	}
});