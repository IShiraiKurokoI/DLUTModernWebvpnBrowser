// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using DLUTModernWebvpnBrowser.Entities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using System.Security.Cryptography;
using System.Text;
using System;
using Windows.Foundation.Metadata;
using Castle.Core.Configuration;
using System.Threading.Tasks;
using DLUTModernWebvpnBrowser.Configurations;
using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.Windows.AppNotifications;
using Castle.Core.Internal;
using System.Diagnostics;
using WinUICommunity;
using Windows.ApplicationModel.DataTransfer;
using System.Net.Http;
using System.Net;
using System.Threading;
using DLUTModernWebvpnBrowser.Helpers;
using DES = DLUTModernWebvpnBrowser.Helpers.DES;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DLUTModernWebvpnBrowser.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// //e711
    public sealed partial class WebViewPage : Page
    {
        private TabViewItem tabViewItem;
        private TabviewPage tabviewPage;
        bool LoginTried = false;
        public WebViewPage()
        {
            this.InitializeComponent();
        }

        public void PrepareConnectedAnimation(ConnectedAnimationConfiguration config)
        {
            var anim = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("ForwardConnectedAnimation", SourceElement);

            if (config != null && ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
            {
                anim.Configuration = config;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TabViewAndItem tabViewAndItem= ((TabViewAndItem)e.Parameter);
            tabViewItem = tabViewAndItem._item;
            tabviewPage = tabViewAndItem._tabview;
            base.OnNavigatedTo(e);

            var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("BackwardConnectedAnimation");
            if (anim != null)
            {
                anim.TryStart(SourceElement);
            }
        }

        private void WebView_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            WebView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = true;
            WebView.CoreWebView2.Settings.IsPasswordAutosaveEnabled = true;
            WebView.CoreWebView2.Settings.IsZoomControlEnabled = true;
            WebView.CoreWebView2.Settings.IsGeneralAutofillEnabled = true;
            WebView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = true;
            WebView.CoreWebView2.NewWindowRequested += (sender, args) =>
            {
                WebView.CoreWebView2.ExecuteScriptAsync("window.location.href='" + args.Uri.ToString() + "'");
                args.Handled = true;
            };
            WebView.CoreWebView2.ContentLoading += (sender, args) =>
            {
                RefreshOrStopIcon.Glyph = "\xe711";
            };
            WebView.CoreWebView2.DOMContentLoaded += (sender, args) =>
            {
                RefreshOrStopIcon.Glyph = "\xe72c";
                tabViewItem.Header = WebView.CoreWebView2.DocumentTitle;
            };
            WebView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
        }

        private void CoreWebView2_NavigationCompleted(CoreWebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            if (WebView.CoreWebView2.DocumentTitle.IndexOf("过期") != -1)
            {
                WebView.ExecuteScriptAsync("document.getElementsByClassName('layui-layer-btn0')[0].click()");
                WebView.ExecuteScriptAsync("document.getElementsByClassName('layui-layer-btn0')[0].click()");
                WebView.ExecuteScriptAsync("document.getElementsByClassName('layui-layer-btn0')[0].click()");
            }
            if (WebView.Source.AbsoluteUri == "https://webvpn.dlut.edu.cn/https/77726476706e69737468656265737421e3e44ed2233c7d44300d8db9d6562d/cas/login?service=https%3A%2F%2Fwebvpn.dlut.edu.cn%2Flogin%3Fcas_login%3Dtrue" || WebView.Source.AbsoluteUri == "https://sso.dlut.edu.cn/cas/login?service=https%3A%2F%2Fwebvpn.dlut.edu.cn%2Flogin%3Fcas_login%3Dtrue"|| WebView.Source.AbsoluteUri.StartsWith("https://webvpn.dlut.edu.cn/https/77726476706e69737468656265737421e3e44ed2233c7d44300d8db9d6562d/cas/login;JSESSIONIDCAS="))
            {
                if(LoginTried)
                {
                    Notify();
                }
                else
                {
                    LoginTried = true;
                    login();
                }
            }
            else
            {
                LoginTried= false;
            }
        }
        async Task Notify()
        {
            var builder = new AppNotificationBuilder()
                .AddText("⚠自动认证失败⚠")
                .AddText("请前往设置界面检查账号密码是否正确");
            var notificationManager = AppNotificationManager.Default;
            notificationManager.Show(builder.BuildNotification());
        }

        async Task login()
        {
            if(string.IsNullOrEmpty(ApplicationConfig.GetSettings("Uid")) || string.IsNullOrEmpty(ApplicationConfig.GetSettings("Password")))
            {
                var builder = new AppNotificationBuilder()
                    .AddText("⚠请先配置账号密码⚠")
                    .AddText("配置完成后刷新原始页面即可自动登录");
                var notificationManager = AppNotificationManager.Default;
                notificationManager.Show(builder.BuildNotification());
                tabviewPage.OpenSetting();
            }
            else
            {
                Task.Run(() =>
                {
                    if (WebvpnKey.Key == null)
                    {
                        string LoginURL = "https://sso.dlut.edu.cn/cas/login?service=https%3A%2F%2Fwebvpn.dlut.edu.cn%2Flogin%3Fcas_login%3Dtrue";
                        var cookieContainer = new CookieContainer();
                        using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer, AllowAutoRedirect = true, UseProxy = false })
                        using (HttpClient client = new HttpClient(handler))
                        {
                            try
                            {
                                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36 Edg/111.0.1661.44");
                                client.Timeout = new TimeSpan(0, 0, 10); // 10是秒数，用于设置超时时长
                                Task<HttpResponseMessage> res = client.GetAsync(LoginURL);
                                string Response = res.Result.Content.ReadAsStringAsync().Result;
                                string JSESSIONIDCAS = res.Result.Headers.ToString().Split("JSESSIONIDCAS=")[1].Split("; path=")[0];
                                string LT = Response.Split("<input type=\"hidden\" id=\"lt\" name=\"lt\" value=\"", StringSplitOptions.None)[1].Split("\"")[0];
                                string execution = Response.Split("<input type=\"hidden\" name=\"execution\" value=\"", StringSplitOptions.None)[1].Split("\"")[0];
                                string RSA = DES.GetRSA(ApplicationConfig.GetSettings("Uid"), ApplicationConfig.GetSettings("Password"), LT);
                                HttpContent content = new StringContent("none=on&rsa=" + RSA + "&ul=" + ApplicationConfig.GetSettings("Uid").Length + "&pl=" + ApplicationConfig.GetSettings("Password").Length + "&sl=0&lt=" + LT + "&execution=" + execution + "&_eventId=submit");
                                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                                cookieContainer.Add(new Uri("https://sso.dlut.edu.cn"), new Cookie("cas_hash", ""));
                                cookieContainer.Add(new Uri("https://sso.dlut.edu.cn"), new Cookie("dlut_cas_un", ApplicationConfig.GetSettings("Uid")));
                                cookieContainer.Add(new Uri("https://sso.dlut.edu.cn"), new Cookie("Language", "zh_CN"));
                                cookieContainer.Add(new Uri("https://sso.dlut.edu.cn"), new Cookie("JSESSIONIDCAS", JSESSIONIDCAS));
                                Task<HttpResponseMessage> res1 = client.PostAsync(LoginURL, content);
                                HttpResponseMessage Response1 = res1.Result;
                                Task<HttpResponseMessage> res2 = client.GetAsync("https://webvpn.dlut.edu.cn/user/info");
                                HttpResponseMessage Response2 = res2.Result;
                                String FinalResponse = Response2.Content.ReadAsStringAsync().Result;
                                Debug.WriteLine(FinalResponse);
                                WebvpnKey.Key = FinalResponse.Split("\"wrdvpnKey\": \"")[1].Split("\"")[0];
                                WebvpnKey.IV = FinalResponse.Split("\"wrdvpnIV\": \"")[1].Split("\"")[0];
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                                var builder = new AppNotificationBuilder()
                                    .AddText(ex.ToString());
                                var notificationManager = AppNotificationManager.Default;
                                notificationManager.Show(builder.BuildNotification());
                            }
                            finally
                            {
                                client.Dispose();
                            }
                        }
                    }
                });
                string jscode = "un.value='" + ApplicationConfig.GetSettings("Uid") + "'";
                string jscode1 = "pd.value='" + ApplicationConfig.GetSettings("Password") + "'";
                string rm = "rememberName.checked='checked'";
                await WebView.CoreWebView2.ExecuteScriptAsync(rm);
                await WebView.CoreWebView2.ExecuteScriptAsync(jscode);
                await WebView.CoreWebView2.ExecuteScriptAsync(jscode1);
                string jscode2 = "login()";
                await WebView.CoreWebView2.ExecuteScriptAsync(jscode2);
            }
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            WebView.GoBack();
        }

        private void RefreshOrStop_Click(object sender, RoutedEventArgs e)
        {
            if(WebView.IsLoaded)
            {
                WebView.CoreWebView2.Reload();
                LoginTried=false;
            }
            else
            {
                WebView.CoreWebView2.Stop();
            }
        }

        private void AddressBox_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                if(!AddressBox.Text.Contains("webvpn.dlut.edu.cn"))
                {
                    WebView.CoreWebView2.ExecuteScriptAsync("window.location.href='" + getvpnurl(AddressBox.Text) + "'");
                }
            }
        }

        string getCiphertext(string hosts)
        {
            string ciphertext = AesEncrypt(hosts, WebvpnKey.Key, WebvpnKey.IV);
            string ciphertextTrunc = ciphertext.Substring(0, hosts.Length * 2);
            return ciphertextTrunc;
        }

        public static string AesEncrypt(string str, string key, string IVString)
        {
            Encoding encoder = Encoding.UTF8;
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);
            RijndaelManaged rm = new RijndaelManaged
            {
                Key = encoder.GetBytes(key),
                Mode = CipherMode.CFB,
                BlockSize = 128,
                Padding = PaddingMode.PKCS7,
                IV = encoder.GetBytes(IVString),
            };
            ICryptoTransform cTransform = rm.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return ToBCDStringLower(resultArray);//result
        }

        public static string ToBCDStringLower(byte[] buffer)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                sb.Append(buffer[i].ToString("x2"));
            }
            return sb.ToString();
        }

        string getvpnurl(string url)
        {
            if (WebvpnKey.Key == null)
            {
                string LoginURL = "https://sso.dlut.edu.cn/cas/login?service=https%3A%2F%2Fwebvpn.dlut.edu.cn%2Flogin%3Fcas_login%3Dtrue";
                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer, AllowAutoRedirect = true, UseProxy = false })
                using (HttpClient client = new HttpClient(handler))
                {
                    try
                    {
                        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36 Edg/111.0.1661.44");
                        client.Timeout = new TimeSpan(0, 0, 10); // 10是秒数，用于设置超时时长
                        Task<HttpResponseMessage> res = client.GetAsync(LoginURL);
                        string Response = res.Result.Content.ReadAsStringAsync().Result;
                        string JSESSIONIDCAS = res.Result.Headers.ToString().Split("JSESSIONIDCAS=")[1].Split("; path=")[0];
                        string LT = Response.Split("<input type=\"hidden\" id=\"lt\" name=\"lt\" value=\"", StringSplitOptions.None)[1].Split("\"")[0];
                        string execution = Response.Split("<input type=\"hidden\" name=\"execution\" value=\"", StringSplitOptions.None)[1].Split("\"")[0];
                        string RSA = DES.GetRSA(ApplicationConfig.GetSettings("Uid"), ApplicationConfig.GetSettings("Password"), LT);
                        HttpContent content = new StringContent("none=on&rsa=" + RSA + "&ul=" + ApplicationConfig.GetSettings("Uid").Length + "&pl=" + ApplicationConfig.GetSettings("Password").Length + "&sl=0&lt=" + LT + "&execution=" + execution + "&_eventId=submit");
                        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                        cookieContainer.Add(new Uri("https://sso.dlut.edu.cn"), new Cookie("cas_hash", ""));
                        cookieContainer.Add(new Uri("https://sso.dlut.edu.cn"), new Cookie("dlut_cas_un", ApplicationConfig.GetSettings("Uid")));
                        cookieContainer.Add(new Uri("https://sso.dlut.edu.cn"), new Cookie("Language", "zh_CN"));
                        cookieContainer.Add(new Uri("https://sso.dlut.edu.cn"), new Cookie("JSESSIONIDCAS", JSESSIONIDCAS));
                        Task<HttpResponseMessage> res1 = client.PostAsync(LoginURL, content);
                        HttpResponseMessage Response1 = res1.Result;
                        Task<HttpResponseMessage> res2 = client.GetAsync("https://webvpn.dlut.edu.cn/user/info");
                        HttpResponseMessage Response2 = res2.Result;
                        String FinalResponse = Response2.Content.ReadAsStringAsync().Result;
                        Debug.WriteLine(FinalResponse);
                        WebvpnKey.Key = FinalResponse.Split("\"wrdvpnKey\": \"")[1].Split("\"")[0];
                        WebvpnKey.IV = FinalResponse.Split("\"wrdvpnIV\": \"")[1].Split("\"")[0];
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        var builder = new AppNotificationBuilder()
                            .AddText(ex.ToString());
                        var notificationManager = AppNotificationManager.Default;
                        notificationManager.Show(builder.BuildNotification());
                    }
                    finally
                    {
                        client.Dispose();
                    }
                }
            }
            string[] parts = url.Split(new[] { "://" }, StringSplitOptions.None);
            string pro = "http";
            string add = "";
            if (parts.Length == 1)
            {
                add = parts[0];
            }
            else
            {
                if (parts[0] == "https")
                {
                    pro = parts[0];
                }
                add = parts[1];
            }
            string[] hosts = add.Split(new[] { "/" }, StringSplitOptions.None);
            string cph = getCiphertext(hosts[0]);
            string fold = "/";
            for (int i = 1; i < hosts.Length; i++)
            {
                fold += ("/" + hosts[i]);
            }
            string final = "https://" + "webvpn.dlut.edu.cn" + '/' + pro + '/' + Convert.ToHexString(Encoding.ASCII.GetBytes(WebvpnKey.IV)) + cph + fold;
            return final;
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            tabviewPage.GetTabView().TabItems.Add(tabviewPage.CreateNewTab(tabviewPage.GetTabView().TabItems.Count));
            tabviewPage.GetTabView().SelectedIndex = tabviewPage.GetTabView().TabItems.Count - 1;
        }

        private void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            tabviewPage.OpenSetting();
        }

        private void MenuFlyoutItem_Click_2(object sender, RoutedEventArgs e)
        {
            tabviewPage.OpenAbout();
        }

        private void MenuFlyoutItem_Click_3(object sender, RoutedEventArgs e)
        {
            tabviewPage.OpenCustom("反馈", "https://github.com/IShiraiKurokoI/DLUTModernWebvpnBrowser/issues");
        }

        private void MenuFlyoutItem_Click_4(object sender, RoutedEventArgs e)
        {
            tabviewPage.OpenCustom("下载", "edge://downloads/all");
        }

        private void MenuFlyoutItem_Click_5(object sender, RoutedEventArgs e)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText("【"+WebView.CoreWebView2.DocumentTitle+"】"+WebView.Source.AbsoluteUri);
            Clipboard.SetContent(dataPackage);
            var builder = new AppNotificationBuilder()
                .AddText("页面链接已复制到剪贴板");
            var notificationManager = AppNotificationManager.Default;
            notificationManager.Show(builder.BuildNotification());
        }

        private void MenuFlyoutItem_Click_6(object sender, RoutedEventArgs e)
        {
            tabviewPage.OpenCustom("历史记录", "edge://history/all");
        }

        private void GoForward_Click(object sender, RoutedEventArgs e)
        {
            WebView.GoForward();
        }
    }
}
