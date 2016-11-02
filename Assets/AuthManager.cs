using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using LitJson;
using System;

public class AuthManager : MonoBehaviour {

    public string m_userKey = "DxI0t2QujeV0nCz90GPfU1QKr3Ma";
    public string m_userSecret = "4NUUhq0dfVEoThJIL2_h3Z8vOZUa";

    public string m_tokenRequestUrl;
	
	// Update is called once per frame
	void Update () {
	
	}

    

    void Start()
    {

        StartCoroutine(Upload());
        
    }

    IEnumerator Upload()
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        formData.Add(new MultipartFormDataSection("grant_type=client_credentials&client_id="+ m_userKey+"&client_secret="+m_userSecret+"&scope=openid"));
        //formData.Add(new MultipartFormDataSection("?grant_type=client_credentials&client_id=DxI0t2QujeV0nCz90GPfU1QKr3Ma&client_secret=4NUUhq0dfVEoThJIL2_h3Z8vOZUa&scope=openid"));

        //formData.Add(new MultipartFormDataSection("grant_type", "client_credentials"));
        //formData.Add(new MultipartFormDataSection("client_id", "DxI0t2QujeV0nCz90GPfU1QKr3Ma"));
        //formData.Add(new MultipartFormDataSection("client_secret", "4NUUhq0dfVEoThJIL2_h3Z8vOZUa"));
        //formData.Add(new MultipartFormDataSection("scope", "openid"));


        UnityWebRequest www = UnityWebRequest.Post("https://api.enco.io/token?grant_type=client_credentials&client_id=" + m_userKey 
                                                                                                                        + "&client_secret=" + m_userSecret 
                                                                                                                        + "&scope=openid", formData);
        yield return www.Send();

        if (www.isError)
        {
            Debug.Log(www.error);
        }
        else
        {

            Debug.Log(www.downloadHandler.text);
            JsonData responseData = JsonMapper.ToObject(www.downloadHandler.text);
            string accessToken = (string)responseData["access_token"];
            print("Access granted: " + accessToken);
            StartCoroutine(GetData(accessToken));
        }
    }
    IEnumerator GetData(string _token)
    {
        //List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        //formData.Add(new MultipartFormDataSection("Authorization", "Bearer 48293aef25046c0fee1e9a76dc4a0196"));
        //formData.Add(new MultipartFormDataSection("Accept", "application/json"));
        //formData.Add(new MultipartFormDataSection("Content-type", ""));
        var headers = new Dictionary<string,string>();
        headers.Add("Authorization", "Bearer " + _token);// 48293aef25046c0fee1e9a76dc4a0196");// );
        headers.Add("Accept", "application/json");
        headers.Add("Content-type", "application/json");
        WWW www = new WWW("https://api.enco.io/seaas/0.0.1/device/DemoRaspberryPi/stream/Temperature/pop", null, headers);
        yield return www;
        print(www.text);
        JsonData data = JsonMapper.ToObject(www.text);
        Debug.Log(data["typedMessage"]["json_payload"]);
        string usefulJson = (string)data["typedMessage"]["json_payload"];

        JsonMapper.RegisterExporter<float>((obj, writer) => writer.Write(Convert.ToDouble(obj)));
        JsonMapper.RegisterImporter<double, float>(input => Convert.ToSingle(input));

        JsonData usefulData = JsonMapper.ToObject(usefulJson);
        int current_temperature = (int)usefulData["temperature"];
        Debug.Log("Current temperature: " + current_temperature.ToString() + " ° C");

    }
}
