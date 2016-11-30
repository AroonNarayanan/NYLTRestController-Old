Imports Newtonsoft.Json
Imports Windows.Storage
Imports Windows.Web.Http
''' <summary>
''' A class for submitting basic CRUD commands to a server, particularly an NYLT CampHub server.
''' </summary>
Public Class RestController

    Private Property Host As String
    Private Property UseLocalStorage As Boolean


    ''' <summary>
    ''' Instantiates REST controller object.
    ''' </summary>
    ''' <param name="Host">The hostname of the REST service.</param>
    Public Sub New(Host As String)
        'instantiate object
        Me.Host = Host
        Me.UseLocalStorage = False
    End Sub

    '''' <summary>
    '''' Instantiates REST controller object.
    '''' </summary>
    '''' <param name="Host">The hostname of the REST service.</param>
    '''' <param name="UseLocalStorage">Whether or not to use offline storage instead of cloud storage.</param>
    'Public Sub New(Host As String, UseLocalStorage As Boolean)
    '    Me.Host = Host
    '    Me.UseLocalStorage = UseLocalStorage
    'End Sub

    ''' <summary>
    ''' Executes a GET command on a given endpoint.
    ''' </summary>
    ''' <typeparam name="T">The type of the expected response.</typeparam>
    ''' <param name="URL">The service endpoint from which to GET the response.</param>
    ''' <returns>The response object.</returns>
    Public Async Function GetObjectAsync(Of T)(URL As String) As Task(Of T)
        Try
            Dim rootFilter As New Filters.HttpBaseProtocolFilter()
            rootFilter.CacheControl.ReadBehavior = Filters.HttpCacheReadBehavior.MostRecent
            rootFilter.CacheControl.WriteBehavior = Filters.HttpCacheWriteBehavior.NoCache
            Dim get_uri = New Uri(Host & URL)
            Dim client As New HttpClient(rootFilter)
            Dim response = Await client.GetAsync(get_uri)
            If response.IsSuccessStatusCode Then
                Dim DeserializedResponse As T = JsonConvert.DeserializeObject(Of T)(Await response.Content.ReadAsStringAsync, New JsonSerializerSettings() With {.DateTimeZoneHandling = DateTimeZoneHandling.Local})
                Return DeserializedResponse
            Else
                Throw New Exception("The server failed to return a successful response.")
            End If
        Catch
            Throw New Exception("The REST controller failed to complete the GET call.  There may not be an internet connection.")
        End Try
    End Function

    ''' <summary>
    ''' Upload a given object via POST command.
    ''' </summary>
    ''' <param name="URL">The service endpoint to which to POST.</param>
    ''' <param name="ObjectToSend">The object to POST.</param>
    ''' <returns>A boolean indicating whether or not the POST response succeeded.</returns>
    Public Async Function PostObjectAsync(URL As String, ObjectToSend As Object) As Task(Of Boolean)
        Try
            Dim post_uri = New Uri(Host & URL)
            Dim client As New HttpClient()
            Dim content As New HttpStringContent(JsonConvert.SerializeObject(ObjectToSend, New Converters.IsoDateTimeConverter()), Streams.UnicodeEncoding.Utf8, "application/json")
            Dim response = Await client.PostAsync(post_uri, content)
            If response.IsSuccessStatusCode Then
                Return True
            Else
                Return False
            End If
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Upload a given object via POST command and get the response.
    ''' </summary>
    ''' <param name="URL">The service endpoint to which to POST.</param>
    ''' <param name="ObjectToSend">The object to POST.</param>
    ''' <returns>The HTTP response if successful - otherwise, null.</returns>
    Public Async Function PostObjectWithResponseAsync(URL As String, ObjectToSend As Object) As Task(Of HttpResponseMessage)
        Try
            Dim post_uri = New Uri(Host & URL)
            Dim client As New HttpClient()
            Dim content As New HttpStringContent(JsonConvert.SerializeObject(ObjectToSend, New Converters.IsoDateTimeConverter()), Streams.UnicodeEncoding.Utf8, "application/json")
            Dim response = Await client.PostAsync(post_uri, content)
            If response.IsSuccessStatusCode Then
                Return response
            Else
                Return Nothing
            End If
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Upload a given scout image to an NYLT CampHub server via POST command.
    ''' </summary>
    ''' <param name="URL">The service endpoint to which to POST.</param>
    ''' <param name="ImageToSend">The scout image to POST.</param>
    ''' <param name="ScoutID">The ID of the scout to whom the image belongs.</param>
    ''' <returns>A boolean indicating whether or not the POST response succeeded.</returns>
    Public Async Function PostScoutImageAsync(URL As String, ImageToSend As StorageFile, ScoutID As Integer) As Task(Of Boolean)
        Dim upload_uri = New Uri(Host & URL)
        Try
            Dim client As New HttpClient()
            Dim formcontent As New HttpMultipartFormDataContent
            Dim imagecontent As New HttpStreamContent(Await ImageToSend.OpenSequentialReadAsync)
            formcontent.Add(imagecontent, "scoutImage", ScoutID)
            Dim response = Await client.PostAsync(upload_uri, formcontent)
            If response.IsSuccessStatusCode Then
                Return True
            Else
                Return False
            End If
        Catch
            Return False
        End Try
    End Function


    ''' <summary>
    ''' Upload a given image to a given URL via form-multipart POST command.
    ''' </summary>
    ''' <param name="URL">The service endpoint to which to POST.</param>
    ''' <param name="ImageToSend">The image to POST.</param>
    ''' <param name="Filename">The name of the file as it is to be stored on the server.</param>
    ''' <param name="FormName">The name of the part containing the file in the form-multipart request body.</param>
    ''' <returns>A boolean indicating whether or not the POST response succeeded.</returns>
    Public Async Function PostImageAsync(URL As String, ImageToSend As StorageFile, Filename As String, FormName As String) As Task(Of Boolean)
        Dim upload_uri = New Uri(Host & URL)
        Try
            Dim client As New HttpClient()
            Dim formcontent As New HttpMultipartFormDataContent
            Dim imagecontent As New HttpStreamContent(Await ImageToSend.OpenSequentialReadAsync)
            formcontent.Add(imagecontent, FormName, Filename)
            Dim response = Await client.PostAsync(upload_uri, formcontent)
            If response.IsSuccessStatusCode Then
                Return True
            Else
                Return False
            End If
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Update a given object via PUT command.
    ''' </summary>
    ''' <param name="URL">The service endpoint to which to PUT.</param>
    ''' <param name="ObjectToSend">The object to PUT.</param>
    ''' <returns>A boolean indicating whether or not the PUT response succeeded.</returns>
    Public Async Function PutObjectAsync(URL As String, ObjectToSend As Object) As Task(Of Boolean)
        Dim put_uri = New Uri(Host & URL)
        Try
            Dim client As New HttpClient()
            Dim content As New HttpStringContent(JsonConvert.SerializeObject(ObjectToSend, New Converters.IsoDateTimeConverter()), Streams.UnicodeEncoding.Utf8, "application/json")
            Dim response = Await client.PutAsync(put_uri, content)
            If response.IsSuccessStatusCode Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Delete a given object via DELETE command.
    ''' </summary>
    ''' <param name="URL">The service endpoint to DELETE.</param>
    ''' <returns>A boolean indicating whether or not the DELETE command succeeded.</returns>
    Public Async Function DeleteObjectAsync(URL As String) As Task(Of Boolean)
        Dim delete_uri = New Uri(Host & URL)
        Try
            Dim client As New HttpClient()
            Dim response = Await client.DeleteAsync(delete_uri)
            If response.IsSuccessStatusCode Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function

End Class