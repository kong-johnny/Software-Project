using AngleSharp.Html.Parser;
using ChakraCore.NET;
// using AngleSharp.Parser.Html;
using CXHttpNS;
using OpenQA.Selenium.DevTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xiaoya.Library.User.Models;

namespace Xiaoya.Library.User
{
    public class LibraryClient
    {
        private const string TEST_USERNAME = "200000000000";
        private const string HEADER_USER_AGENT = "User-Agent";
        private const string USER_AGENT =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) " +
            "Chrome/55.0.2883.87 Safari/537.36";

        public string Username { get; set; }
        public string Password { get; set; }

        private CXSession m_Session = new CXSession();

        private string m_Key = "";
        private HtmlParser m_Parser = new HtmlParser();

        private const string URL_PREFIX = "http://opac.lib.bnu.edu.cn:8080";
        // private const string URL_PREFIX = "https://bnu.primo.exlibrisgroup.com.cn/";
       // private const string URL_PREFIX = "https://bnu.primo.exlibrisgroup.com.cn/discovery/account?vid=86BNU_INST:86BNU&lang=zh&section=overview";
        private const string URL_LOGIN_GET = URL_PREFIX + "/F/?func=bor-loan&adm_library=";
        private const string URL_LOGIN_POST = URL_PREFIX + "/pds";
        private const string URL_LOGIN = "https://cas.bnu.edu.cn/cas/login?service=https%3A%2F%2Fbnu.primo.exlibrisgroup.com.cn%2Finfra%2FcasRedirect?ctx=/primaws";
        private const string URL_LOGIN_JUMP = URL_PREFIX + "/F/{0}?func=bor-info";
        private const string URL_BORROWED_BOOKS = URL_PREFIX + "/F/{0}?func=bor-loan&adm_library=BNU51";
        private const string URL_RENEW_ALL = URL_PREFIX + "/F/{0}?func=bor-renew-all&adm_library=BNU51";
        public LibraryClient() { }

        public LibraryClient(string username, string password)
        {
            Username = username;
            Password = password;
        }

        private async Task<string[]> FetchLoginParams()
        {
            string lt = "", exec = "";
            string[] retParam = new string[3];
            try
            {
                var res = await m_Session.Req                       // GET URL_LOGIN with specific UA
                   .Url(URL_LOGIN)
                   .ClearCookies()
                   .Header(HEADER_USER_AGENT, USER_AGENT)
                   .Get();

                string body = await res.Content();                  // fetch the response body (html)


                // var doc = m_Parser.Parse(body);
                var doc = m_Parser.ParseDocument(body);
                var ltElement = doc.GetElementsByName("lt");
                if (ltElement.Count() > 0)
                {
                    lt = ltElement.First().GetAttribute("value");
                }

                var execElement = doc.GetElementsByName("execution");
                if (execElement.Count() > 0)
                {
                    exec = execElement.First().GetAttribute("value");
                }


                ChakraRuntime runtime = ChakraRuntime.Create();
                ChakraContext context = runtime.CreateContext(true);
                context.RunScript("function strEnc(data,firstKey,secondKey,thirdKey){var leng=data.length;var encData=\"\";var firstKeyBt,secondKeyBt,thirdKeyBt,firstLength,secondLength,thirdLength;if(firstKey!=null&&firstKey!=\"\"){firstKeyBt=getKeyBytes(firstKey);firstLength=firstKeyBt.length;}\r\nif(secondKey!=null&&secondKey!=\"\"){secondKeyBt=getKeyBytes(secondKey);secondLength=secondKeyBt.length;}\r\nif(thirdKey!=null&&thirdKey!=\"\"){thirdKeyBt=getKeyBytes(thirdKey);thirdLength=thirdKeyBt.length;}\r\nif(leng>0){if(leng<4){var bt=strToBt(data);var encByte;if(firstKey!=null&&firstKey!=\"\"&&secondKey!=null&&secondKey!=\"\"&&thirdKey!=null&&thirdKey!=\"\"){var tempBt;var x,y,z;tempBt=bt;for(x=0;x<firstLength;x++){tempBt=enc(tempBt,firstKeyBt[x]);}\r\nfor(y=0;y<secondLength;y++){tempBt=enc(tempBt,secondKeyBt[y]);}\r\nfor(z=0;z<thirdLength;z++){tempBt=enc(tempBt,thirdKeyBt[z]);}\r\nencByte=tempBt;}else{if(firstKey!=null&&firstKey!=\"\"&&secondKey!=null&&secondKey!=\"\"){var tempBt;var x,y;tempBt=bt;for(x=0;x<firstLength;x++){tempBt=enc(tempBt,firstKeyBt[x]);}\r\nfor(y=0;y<secondLength;y++){tempBt=enc(tempBt,secondKeyBt[y]);}\r\nencByte=tempBt;}else{if(firstKey!=null&&firstKey!=\"\"){var tempBt;var x=0;tempBt=bt;for(x=0;x<firstLength;x++){tempBt=enc(tempBt,firstKeyBt[x]);}\r\nencByte=tempBt;}}}\r\nencData=bt64ToHex(encByte);}else{var iterator=parseInt(leng/4);var remainder=leng%4;var i=0;for(i=0;i<iterator;i++){var tempData=data.substring(i*4+0,i*4+4);var tempByte=strToBt(tempData);var encByte;if(firstKey!=null&&firstKey!=\"\"&&secondKey!=null&&secondKey!=\"\"&&thirdKey!=null&&thirdKey!=\"\"){var tempBt;var x,y,z;tempBt=tempByte;for(x=0;x<firstLength;x++){tempBt=enc(tempBt,firstKeyBt[x]);}\r\nfor(y=0;y<secondLength;y++){tempBt=enc(tempBt,secondKeyBt[y]);}\r\nfor(z=0;z<thirdLength;z++){tempBt=enc(tempBt,thirdKeyBt[z]);}\r\nencByte=tempBt;}else{if(firstKey!=null&&firstKey!=\"\"&&secondKey!=null&&secondKey!=\"\"){var tempBt;var x,y;tempBt=tempByte;for(x=0;x<firstLength;x++){tempBt=enc(tempBt,firstKeyBt[x]);}\r\nfor(y=0;y<secondLength;y++){tempBt=enc(tempBt,secondKeyBt[y]);}\r\nencByte=tempBt;}else{if(firstKey!=null&&firstKey!=\"\"){var tempBt;var x;tempBt=tempByte;for(x=0;x<firstLength;x++){tempBt=enc(tempBt,firstKeyBt[x]);}\r\nencByte=tempBt;}}}\r\nencData+=bt64ToHex(encByte);}\r\nif(remainder>0){var remainderData=data.substring(iterator*4+0,leng);var tempByte=strToBt(remainderData);var encByte;if(firstKey!=null&&firstKey!=\"\"&&secondKey!=null&&secondKey!=\"\"&&thirdKey!=null&&thirdKey!=\"\"){var tempBt;var x,y,z;tempBt=tempByte;for(x=0;x<firstLength;x++){tempBt=enc(tempBt,firstKeyBt[x]);}\r\nfor(y=0;y<secondLength;y++){tempBt=enc(tempBt,secondKeyBt[y]);}\r\nfor(z=0;z<thirdLength;z++){tempBt=enc(tempBt,thirdKeyBt[z]);}\r\nencByte=tempBt;}else{if(firstKey!=null&&firstKey!=\"\"&&secondKey!=null&&secondKey!=\"\"){var tempBt;var x,y;tempBt=tempByte;for(x=0;x<firstLength;x++){tempBt=enc(tempBt,firstKeyBt[x]);}\r\nfor(y=0;y<secondLength;y++){tempBt=enc(tempBt,secondKeyBt[y]);}\r\nencByte=tempBt;}else{if(firstKey!=null&&firstKey!=\"\"){var tempBt;var x;tempBt=tempByte;for(x=0;x<firstLength;x++){tempBt=enc(tempBt,firstKeyBt[x]);}\r\nencByte=tempBt;}}}\r\nencData+=bt64ToHex(encByte);}}}\r\nreturn encData;}\r\nfunction strDec(data,firstKey,secondKey,thirdKey){var leng=data.length;var decStr=\"\";var firstKeyBt,secondKeyBt,thirdKeyBt,firstLength,secondLength,thirdLength;if(firstKey!=null&&firstKey!=\"\"){firstKeyBt=getKeyBytes(firstKey);firstLength=firstKeyBt.length;}\r\nif(secondKey!=null&&secondKey!=\"\"){secondKeyBt=getKeyBytes(secondKey);secondLength=secondKeyBt.length;}\r\nif(thirdKey!=null&&thirdKey!=\"\"){thirdKeyBt=getKeyBytes(thirdKey);thirdLength=thirdKeyBt.length;}\r\nvar iterator=parseInt(leng/16);var i=0;for(i=0;i<iterator;i++){var tempData=data.substring(i*16+0,i*16+16);var strByte=hexToBt64(tempData);var intByte=new Array(64);var j=0;for(j=0;j<64;j++){intByte[j]=parseInt(strByte.substring(j,j+1));}\r\nvar decByte;if(firstKey!=null&&firstKey!=\"\"&&secondKey!=null&&secondKey!=\"\"&&thirdKey!=null&&thirdKey!=\"\"){var tempBt;var x,y,z;tempBt=intByte;for(x=thirdLength-1;x>=0;x--){tempBt=dec(tempBt,thirdKeyBt[x]);}\r\nfor(y=secondLength-1;y>=0;y--){tempBt=dec(tempBt,secondKeyBt[y]);}\r\nfor(z=firstLength-1;z>=0;z--){tempBt=dec(tempBt,firstKeyBt[z]);}\r\ndecByte=tempBt;}else{if(firstKey!=null&&firstKey!=\"\"&&secondKey!=null&&secondKey!=\"\"){var tempBt;var x,y,z;tempBt=intByte;for(x=secondLength-1;x>=0;x--){tempBt=dec(tempBt,secondKeyBt[x]);}\r\nfor(y=firstLength-1;y>=0;y--){tempBt=dec(tempBt,firstKeyBt[y]);}\r\ndecByte=tempBt;}else{if(firstKey!=null&&firstKey!=\"\"){var tempBt;var x,y,z;tempBt=intByte;for(x=firstLength-1;x>=0;x--){tempBt=dec(tempBt,firstKeyBt[x]);}\r\ndecByte=tempBt;}}}\r\ndecStr+=byteToString(decByte);}\r\nreturn decStr;}\r\nfunction getKeyBytes(key){var keyBytes=new Array();var leng=key.length;var iterator=parseInt(leng/4);var remainder=leng%4;var i=0;for(i=0;i<iterator;i++){keyBytes[i]=strToBt(key.substring(i*4+0,i*4+4));}\r\nif(remainder>0){keyBytes[i]=strToBt(key.substring(i*4+0,leng));}\r\nreturn keyBytes;}\r\nfunction strToBt(str){var leng=str.length;var bt=new Array(64);if(leng<4){var i=0,j=0,p=0,q=0;for(i=0;i<leng;i++){var k=str.charCodeAt(i);for(j=0;j<16;j++){var pow=1,m=0;for(m=15;m>j;m--){pow*=2;}\r\nbt[16*i+j]=parseInt(k/pow)%2;}}\r\nfor(p=leng;p<4;p++){var k=0;for(q=0;q<16;q++){var pow=1,m=0;for(m=15;m>q;m--){pow*=2;}\r\nbt[16*p+q]=parseInt(k/pow)%2;}}}else{for(i=0;i<4;i++){var k=str.charCodeAt(i);for(j=0;j<16;j++){var pow=1;for(m=15;m>j;m--){pow*=2;}\r\nbt[16*i+j]=parseInt(k/pow)%2;}}}\r\nreturn bt;}\r\nfunction bt4ToHex(binary){var hex;switch(binary){case\"0000\":hex=\"0\";break;case\"0001\":hex=\"1\";break;case\"0010\":hex=\"2\";break;case\"0011\":hex=\"3\";break;case\"0100\":hex=\"4\";break;case\"0101\":hex=\"5\";break;case\"0110\":hex=\"6\";break;case\"0111\":hex=\"7\";break;case\"1000\":hex=\"8\";break;case\"1001\":hex=\"9\";break;case\"1010\":hex=\"A\";break;case\"1011\":hex=\"B\";break;case\"1100\":hex=\"C\";break;case\"1101\":hex=\"D\";break;case\"1110\":hex=\"E\";break;case\"1111\":hex=\"F\";break;}\r\nreturn hex;}\r\nfunction hexToBt4(hex){var binary;switch(hex){case\"0\":binary=\"0000\";break;case\"1\":binary=\"0001\";break;case\"2\":binary=\"0010\";break;case\"3\":binary=\"0011\";break;case\"4\":binary=\"0100\";break;case\"5\":binary=\"0101\";break;case\"6\":binary=\"0110\";break;case\"7\":binary=\"0111\";break;case\"8\":binary=\"1000\";break;case\"9\":binary=\"1001\";break;case\"A\":binary=\"1010\";break;case\"B\":binary=\"1011\";break;case\"C\":binary=\"1100\";break;case\"D\":binary=\"1101\";break;case\"E\":binary=\"1110\";break;case\"F\":binary=\"1111\";break;}\r\nreturn binary;}\r\nfunction byteToString(byteData){var str=\"\";for(i=0;i<4;i++){var count=0;for(j=0;j<16;j++){var pow=1;for(m=15;m>j;m--){pow*=2;}\r\ncount+=byteData[16*i+j]*pow;}\r\nif(count!=0){str+=String.fromCharCode(count);}}\r\nreturn str;}\r\nfunction bt64ToHex(byteData){var hex=\"\";for(i=0;i<16;i++){var bt=\"\";for(j=0;j<4;j++){bt+=byteData[i*4+j];}\r\nhex+=bt4ToHex(bt);}\r\nreturn hex;}\r\nfunction hexToBt64(hex){var binary=\"\";for(i=0;i<16;i++){binary+=hexToBt4(hex.substring(i,i+1));}\r\nreturn binary;}\r\nfunction enc(dataByte,keyByte){var keys=generateKeys(keyByte);var ipByte=initPermute(dataByte);var ipLeft=new Array(32);var ipRight=new Array(32);var tempLeft=new Array(32);var i=0,j=0,k=0,m=0,n=0;for(k=0;k<32;k++){ipLeft[k]=ipByte[k];ipRight[k]=ipByte[32+k];}\r\nfor(i=0;i<16;i++){for(j=0;j<32;j++){tempLeft[j]=ipLeft[j];ipLeft[j]=ipRight[j];}\r\nvar key=new Array(48);for(m=0;m<48;m++){key[m]=keys[i][m];}\r\nvar tempRight=xor(pPermute(sBoxPermute(xor(expandPermute(ipRight),key))),tempLeft);for(n=0;n<32;n++){ipRight[n]=tempRight[n];}}\r\nvar finalData=new Array(64);for(i=0;i<32;i++){finalData[i]=ipRight[i];finalData[32+i]=ipLeft[i];}\r\nreturn finallyPermute(finalData);}\r\nfunction dec(dataByte,keyByte){var keys=generateKeys(keyByte);var ipByte=initPermute(dataByte);var ipLeft=new Array(32);var ipRight=new Array(32);var tempLeft=new Array(32);var i=0,j=0,k=0,m=0,n=0;for(k=0;k<32;k++){ipLeft[k]=ipByte[k];ipRight[k]=ipByte[32+k];}\r\nfor(i=15;i>=0;i--){for(j=0;j<32;j++){tempLeft[j]=ipLeft[j];ipLeft[j]=ipRight[j];}\r\nvar key=new Array(48);for(m=0;m<48;m++){key[m]=keys[i][m];}\r\nvar tempRight=xor(pPermute(sBoxPermute(xor(expandPermute(ipRight),key))),tempLeft);for(n=0;n<32;n++){ipRight[n]=tempRight[n];}}\r\nvar finalData=new Array(64);for(i=0;i<32;i++){finalData[i]=ipRight[i];finalData[32+i]=ipLeft[i];}\r\nreturn finallyPermute(finalData);}\r\nfunction initPermute(originalData){var ipByte=new Array(64);for(i=0,m=1,n=0;i<4;i++,m+=2,n+=2){for(j=7,k=0;j>=0;j--,k++){ipByte[i*8+k]=originalData[j*8+m];ipByte[i*8+k+32]=originalData[j*8+n];}}\r\nreturn ipByte;}\r\nfunction expandPermute(rightData){var epByte=new Array(48);for(i=0;i<8;i++){if(i==0){epByte[i*6+0]=rightData[31];}else{epByte[i*6+0]=rightData[i*4-1];}\r\nepByte[i*6+1]=rightData[i*4+0];epByte[i*6+2]=rightData[i*4+1];epByte[i*6+3]=rightData[i*4+2];epByte[i*6+4]=rightData[i*4+3];if(i==7){epByte[i*6+5]=rightData[0];}else{epByte[i*6+5]=rightData[i*4+4];}}\r\nreturn epByte;}\r\nfunction xor(byteOne,byteTwo){var xorByte=new Array(byteOne.length);for(i=0;i<byteOne.length;i++){xorByte[i]=byteOne[i]^byteTwo[i];}\r\nreturn xorByte;}\r\nfunction sBoxPermute(expandByte){var sBoxByte=new Array(32);var binary=\"\";var s1=[[14,4,13,1,2,15,11,8,3,10,6,12,5,9,0,7],[0,15,7,4,14,2,13,1,10,6,12,11,9,5,3,8],[4,1,14,8,13,6,2,11,15,12,9,7,3,10,5,0],[15,12,8,2,4,9,1,7,5,11,3,14,10,0,6,13]];var s2=[[15,1,8,14,6,11,3,4,9,7,2,13,12,0,5,10],[3,13,4,7,15,2,8,14,12,0,1,10,6,9,11,5],[0,14,7,11,10,4,13,1,5,8,12,6,9,3,2,15],[13,8,10,1,3,15,4,2,11,6,7,12,0,5,14,9]];var s3=[[10,0,9,14,6,3,15,5,1,13,12,7,11,4,2,8],[13,7,0,9,3,4,6,10,2,8,5,14,12,11,15,1],[13,6,4,9,8,15,3,0,11,1,2,12,5,10,14,7],[1,10,13,0,6,9,8,7,4,15,14,3,11,5,2,12]];var s4=[[7,13,14,3,0,6,9,10,1,2,8,5,11,12,4,15],[13,8,11,5,6,15,0,3,4,7,2,12,1,10,14,9],[10,6,9,0,12,11,7,13,15,1,3,14,5,2,8,4],[3,15,0,6,10,1,13,8,9,4,5,11,12,7,2,14]];var s5=[[2,12,4,1,7,10,11,6,8,5,3,15,13,0,14,9],[14,11,2,12,4,7,13,1,5,0,15,10,3,9,8,6],[4,2,1,11,10,13,7,8,15,9,12,5,6,3,0,14],[11,8,12,7,1,14,2,13,6,15,0,9,10,4,5,3]];var s6=[[12,1,10,15,9,2,6,8,0,13,3,4,14,7,5,11],[10,15,4,2,7,12,9,5,6,1,13,14,0,11,3,8],[9,14,15,5,2,8,12,3,7,0,4,10,1,13,11,6],[4,3,2,12,9,5,15,10,11,14,1,7,6,0,8,13]];var s7=[[4,11,2,14,15,0,8,13,3,12,9,7,5,10,6,1],[13,0,11,7,4,9,1,10,14,3,5,12,2,15,8,6],[1,4,11,13,12,3,7,14,10,15,6,8,0,5,9,2],[6,11,13,8,1,4,10,7,9,5,0,15,14,2,3,12]];var s8=[[13,2,8,4,6,15,11,1,10,9,3,14,5,0,12,7],[1,15,13,8,10,3,7,4,12,5,6,11,0,14,9,2],[7,11,4,1,9,12,14,2,0,6,10,13,15,3,5,8],[2,1,14,7,4,10,8,13,15,12,9,0,3,5,6,11]];for(m=0;m<8;m++){var i=0,j=0;i=expandByte[m*6+0]*2+expandByte[m*6+5];j=expandByte[m*6+1]*2*2*2\r\n+expandByte[m*6+2]*2*2\r\n+expandByte[m*6+3]*2\r\n+expandByte[m*6+4];switch(m){case 0:binary=getBoxBinary(s1[i][j]);break;case 1:binary=getBoxBinary(s2[i][j]);break;case 2:binary=getBoxBinary(s3[i][j]);break;case 3:binary=getBoxBinary(s4[i][j]);break;case 4:binary=getBoxBinary(s5[i][j]);break;case 5:binary=getBoxBinary(s6[i][j]);break;case 6:binary=getBoxBinary(s7[i][j]);break;case 7:binary=getBoxBinary(s8[i][j]);break;}\r\nsBoxByte[m*4+0]=parseInt(binary.substring(0,1));sBoxByte[m*4+1]=parseInt(binary.substring(1,2));sBoxByte[m*4+2]=parseInt(binary.substring(2,3));sBoxByte[m*4+3]=parseInt(binary.substring(3,4));}\r\nreturn sBoxByte;}\r\nfunction pPermute(sBoxByte){var pBoxPermute=new Array(32);pBoxPermute[0]=sBoxByte[15];pBoxPermute[1]=sBoxByte[6];pBoxPermute[2]=sBoxByte[19];pBoxPermute[3]=sBoxByte[20];pBoxPermute[4]=sBoxByte[28];pBoxPermute[5]=sBoxByte[11];pBoxPermute[6]=sBoxByte[27];pBoxPermute[7]=sBoxByte[16];pBoxPermute[8]=sBoxByte[0];pBoxPermute[9]=sBoxByte[14];pBoxPermute[10]=sBoxByte[22];pBoxPermute[11]=sBoxByte[25];pBoxPermute[12]=sBoxByte[4];pBoxPermute[13]=sBoxByte[17];pBoxPermute[14]=sBoxByte[30];pBoxPermute[15]=sBoxByte[9];pBoxPermute[16]=sBoxByte[1];pBoxPermute[17]=sBoxByte[7];pBoxPermute[18]=sBoxByte[23];pBoxPermute[19]=sBoxByte[13];pBoxPermute[20]=sBoxByte[31];pBoxPermute[21]=sBoxByte[26];pBoxPermute[22]=sBoxByte[2];pBoxPermute[23]=sBoxByte[8];pBoxPermute[24]=sBoxByte[18];pBoxPermute[25]=sBoxByte[12];pBoxPermute[26]=sBoxByte[29];pBoxPermute[27]=sBoxByte[5];pBoxPermute[28]=sBoxByte[21];pBoxPermute[29]=sBoxByte[10];pBoxPermute[30]=sBoxByte[3];pBoxPermute[31]=sBoxByte[24];return pBoxPermute;}\r\nfunction finallyPermute(endByte){var fpByte=new Array(64);fpByte[0]=endByte[39];fpByte[1]=endByte[7];fpByte[2]=endByte[47];fpByte[3]=endByte[15];fpByte[4]=endByte[55];fpByte[5]=endByte[23];fpByte[6]=endByte[63];fpByte[7]=endByte[31];fpByte[8]=endByte[38];fpByte[9]=endByte[6];fpByte[10]=endByte[46];fpByte[11]=endByte[14];fpByte[12]=endByte[54];fpByte[13]=endByte[22];fpByte[14]=endByte[62];fpByte[15]=endByte[30];fpByte[16]=endByte[37];fpByte[17]=endByte[5];fpByte[18]=endByte[45];fpByte[19]=endByte[13];fpByte[20]=endByte[53];fpByte[21]=endByte[21];fpByte[22]=endByte[61];fpByte[23]=endByte[29];fpByte[24]=endByte[36];fpByte[25]=endByte[4];fpByte[26]=endByte[44];fpByte[27]=endByte[12];fpByte[28]=endByte[52];fpByte[29]=endByte[20];fpByte[30]=endByte[60];fpByte[31]=endByte[28];fpByte[32]=endByte[35];fpByte[33]=endByte[3];fpByte[34]=endByte[43];fpByte[35]=endByte[11];fpByte[36]=endByte[51];fpByte[37]=endByte[19];fpByte[38]=endByte[59];fpByte[39]=endByte[27];fpByte[40]=endByte[34];fpByte[41]=endByte[2];fpByte[42]=endByte[42];fpByte[43]=endByte[10];fpByte[44]=endByte[50];fpByte[45]=endByte[18];fpByte[46]=endByte[58];fpByte[47]=endByte[26];fpByte[48]=endByte[33];fpByte[49]=endByte[1];fpByte[50]=endByte[41];fpByte[51]=endByte[9];fpByte[52]=endByte[49];fpByte[53]=endByte[17];fpByte[54]=endByte[57];fpByte[55]=endByte[25];fpByte[56]=endByte[32];fpByte[57]=endByte[0];fpByte[58]=endByte[40];fpByte[59]=endByte[8];fpByte[60]=endByte[48];fpByte[61]=endByte[16];fpByte[62]=endByte[56];fpByte[63]=endByte[24];return fpByte;}\r\nfunction getBoxBinary(i){var binary=\"\";switch(i){case 0:binary=\"0000\";break;case 1:binary=\"0001\";break;case 2:binary=\"0010\";break;case 3:binary=\"0011\";break;case 4:binary=\"0100\";break;case 5:binary=\"0101\";break;case 6:binary=\"0110\";break;case 7:binary=\"0111\";break;case 8:binary=\"1000\";break;case 9:binary=\"1001\";break;case 10:binary=\"1010\";break;case 11:binary=\"1011\";break;case 12:binary=\"1100\";break;case 13:binary=\"1101\";break;case 14:binary=\"1110\";break;case 15:binary=\"1111\";break;}\r\nreturn binary;}\r\nfunction generateKeys(keyByte){var key=new Array(56);var keys=new Array();keys[0]=new Array();keys[1]=new Array();keys[2]=new Array();keys[3]=new Array();keys[4]=new Array();keys[5]=new Array();keys[6]=new Array();keys[7]=new Array();keys[8]=new Array();keys[9]=new Array();keys[10]=new Array();keys[11]=new Array();keys[12]=new Array();keys[13]=new Array();keys[14]=new Array();keys[15]=new Array();var loop=[1,1,2,2,2,2,2,2,1,2,2,2,2,2,2,1];for(i=0;i<7;i++){for(j=0,k=7;j<8;j++,k--){key[i*8+j]=keyByte[8*k+i];}}\r\nvar i=0;for(i=0;i<16;i++){var tempLeft=0;var tempRight=0;for(j=0;j<loop[i];j++){tempLeft=key[0];tempRight=key[28];for(k=0;k<27;k++){key[k]=key[k+1];key[28+k]=key[29+k];}\r\nkey[27]=tempLeft;key[55]=tempRight;}\r\nvar tempKey=new Array(48);tempKey[0]=key[13];tempKey[1]=key[16];tempKey[2]=key[10];tempKey[3]=key[23];tempKey[4]=key[0];tempKey[5]=key[4];tempKey[6]=key[2];tempKey[7]=key[27];tempKey[8]=key[14];tempKey[9]=key[5];tempKey[10]=key[20];tempKey[11]=key[9];tempKey[12]=key[22];tempKey[13]=key[18];tempKey[14]=key[11];tempKey[15]=key[3];tempKey[16]=key[25];tempKey[17]=key[7];tempKey[18]=key[15];tempKey[19]=key[6];tempKey[20]=key[26];tempKey[21]=key[19];tempKey[22]=key[12];tempKey[23]=key[1];tempKey[24]=key[40];tempKey[25]=key[51];tempKey[26]=key[30];tempKey[27]=key[36];tempKey[28]=key[46];tempKey[29]=key[54];tempKey[30]=key[29];tempKey[31]=key[39];tempKey[32]=key[50];tempKey[33]=key[44];tempKey[34]=key[32];tempKey[35]=key[47];tempKey[36]=key[43];tempKey[37]=key[48];tempKey[38]=key[38];tempKey[39]=key[55];tempKey[40]=key[33];tempKey[41]=key[52];tempKey[42]=key[45];tempKey[43]=key[41];tempKey[44]=key[49];tempKey[45]=key[35];tempKey[46]=key[28];tempKey[47]=key[31];switch(i){case 0:for(m=0;m<48;m++){keys[0][m]=tempKey[m];}break;case 1:for(m=0;m<48;m++){keys[1][m]=tempKey[m];}break;case 2:for(m=0;m<48;m++){keys[2][m]=tempKey[m];}break;case 3:for(m=0;m<48;m++){keys[3][m]=tempKey[m];}break;case 4:for(m=0;m<48;m++){keys[4][m]=tempKey[m];}break;case 5:for(m=0;m<48;m++){keys[5][m]=tempKey[m];}break;case 6:for(m=0;m<48;m++){keys[6][m]=tempKey[m];}break;case 7:for(m=0;m<48;m++){keys[7][m]=tempKey[m];}break;case 8:for(m=0;m<48;m++){keys[8][m]=tempKey[m];}break;case 9:for(m=0;m<48;m++){keys[9][m]=tempKey[m];}break;case 10:for(m=0;m<48;m++){keys[10][m]=tempKey[m];}break;case 11:for(m=0;m<48;m++){keys[11][m]=tempKey[m];}break;case 12:for(m=0;m<48;m++){keys[12][m]=tempKey[m];}break;case 13:for(m=0;m<48;m++){keys[13][m]=tempKey[m];}break;case 14:for(m=0;m<48;m++){keys[14][m]=tempKey[m];}break;case 15:for(m=0;m<48;m++){keys[15][m]=tempKey[m];}break;}}\r\nreturn keys;}\r\nfunction run(u,p,lt){return strEnc(u+p+lt,'1','2','3');}");
                retParam[0] = context.GlobalObject.CallFunction<string, string, string, string>("run", Username, Password, lt);
                retParam[1] = lt;
                retParam[2] = exec;
            }
            catch (Exception e)
            {
                retParam[0] = retParam[1] = retParam[2] = "";
            }
            return retParam;
        }

        public async Task<string> Login()
        {
            // TEST

            if (Username == TEST_USERNAME)
            {
                return null;
            }

            // TEST
            m_Key = "";
            try
            {
                var res = await m_Session.Req
                    .Url(URL_LOGIN_GET)
                    .ClearCookies()
                    .Get();

                var body = await res.Content("UTF-8");

                Match mc = Regex.Match(body, URL_PREFIX + "/F/(.*?)\\?func");
                m_Key = mc.Groups[1].Value;

                /*res = await m_Session.Req
                    .Url(URL_LOGIN_POST)
                    .Data("func", "login")
                    .Data("calling_system", "bnuhome")
                    .Data("term1", "short")
                    .Data("selfreg", "")
                    .Data("bor_id", Username)
                    .Data("bor_verification", Password)
                    .Data("url", String.Format(URL_LOGIN_JUMP, m_Key))
                    .Post();*/
                string[] loginParams = await FetchLoginParams();
                res = await m_Session.Req
                .Url(URL_LOGIN)
                    .Header(HEADER_USER_AGENT, USER_AGENT)
                    // .Data("username", m_Username)
                    // .Data("password", m_Password)
                    .Data("code", "code")
                    .Data("ul", "" + Username.Length)
                    .Data("pl", "" + Password.Length)
                    .Data("rsa", loginParams[0])
                    .Data("lt", loginParams[1])
                    .Data("execution", loginParams[2])
                    .Data("_eventId", "submit")
                    .Post();

                body = await res.Content("UTF-8");

                mc = Regex.Match(body, "href=\"(.*?)\">Click");
                if (mc.Success)
                {
                    string urlLogin = mc.Groups[1].Value;
                    await m_Session.Req
                        .Url(URL_PREFIX + urlLogin)
                        .Get();
                    return null;
                }
                else
                {
                    return "用户名密码错误";
                }
            }
            catch
            {
                return "网络错误";
            }
        }

        public async Task<List<BorrowedBook>> GetBorrowedBooks()
        {
            // TEST

            if (Username == TEST_USERNAME)
            {
                return new List<BorrowedBook>()
                {
                    new BorrowedBook("西游记", "吴承恩", "2017-7-12", "", "主馆", "I120-2", "")
                };
            }

            // TEST
            var books = new List<BorrowedBook>();
            try
            {
                var res = await m_Session.Req
                    .Url(String.Format(URL_BORROWED_BOOKS, m_Key))
                    .Get();

                var body = await res.Content("UTF-8");
                // var doc = m_Parser.Parse(body);
                var doc = m_Parser.ParseDocument(body);

                var tables = doc.GetElementsByTagName("table");
                foreach (var table in tables)
                {
                    var text3 = table.GetElementsByClassName("text3");
                    if (text3.Length > 0 &&
                        text3[0].TextContent != null &&
                        text3[0].TextContent.Contains("No."))
                    {
                        foreach (var tr in table.GetElementsByTagName("tr"))
                        {
                            if (tr.ClassName == "tr1")
                            {
                                continue;
                            }
                            var td = tr.GetElementsByTagName("td");
                            if (td.Length > 9)
                            {
                                books.Add(new BorrowedBook(
                                    title: td[3].TextContent,
                                    author: td[2].TextContent,
                                    returnDate: td[5].TextContent,
                                    fine: td[6].TextContent,
                                    building: td[7].TextContent,
                                    position: td[8].TextContent,
                                    description: td[9].TextContent
                                ));
                            }
                        }
                        break;
                    }
                }

            }
            catch
            {
                books.Add(new BorrowedBook("网络错误", "", "", "", "", "", ""));
            }
            return books;
        }

        public async Task<string> RenewAll()
        {
            // TEST

            if (Username == TEST_USERNAME)
            {
                return "已续约过，续约失败";
            }

            // TEST
            try
            {
                var res = await m_Session.Req
                    .Url(String.Format(URL_RENEW_ALL, m_Key))
                    .Get();

                var body = await res.Content("UTF-8");
                // var doc = m_Parser.Parse(body);
                var doc = m_Parser.ParseDocument(body);

                var title = doc.GetElementsByClassName("title");
                if (title.Length == 0)
                {
                    return "未知错误";
                }
                else
                {
                    var info = title[title.Length - 1].TextContent.Trim() + "\n\n";
                    var tables = doc.GetElementsByTagName("table");
                    if (tables.Length > 4)
                    {
                        var table = tables[4];
                        foreach (var tr in table.GetElementsByTagName("tr"))
                        {
                            if (tr.ClassName == "tr1")
                            {
                                continue;
                            }
                            var td = tr.GetElementsByTagName("td");
                            if (td.Length > 8)
                            {
                                info += Convert.ToString(td[1].TextContent) +
                                    ": " + Convert.ToString(td[8].TextContent) + "\n";
                            }
                        }
                    }
                    return info;
                }
            }
            catch
            {
                return "网络错误";
            }

        }
    }
}
