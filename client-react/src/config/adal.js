/* global fetch */

import { AuthenticationContext, adalFetch, withAdalLogin } from 'react-adal'

export const adalConfig = {
  tenant: 'tenantid',
  clientId: 'clientid',
  endpoints: {
    api: 'resource'
  },
  apiUrl: 'api base url',
  cacheLocation: 'localStorage'
}

export const authContext = new AuthenticationContext(adalConfig)

export const adalApiFetch = (api, options) =>
  adalFetch(authContext, adalConfig.endpoints.api, fetch, `${adalConfig.apiUrl}/${api}`, options)

export const withAdalLoginApi = withAdalLogin(
  authContext,
  adalConfig.endpoints.api
)

export const adalUserInfo = () => authContext.getCachedUser()

export const getToken = () =>  authContext.getCachedToken(adalConfig.clientId);


export const logOutUrl = `https://login.windows.net/${adalConfig.tenant}/oauth2/logout?post_logout_redirect_uri=http://localhost:3000/`


// export function azureApiRequest(method, resource, body) {
//     var resourceUrl = `${adalConfig.apiUrl}` + resource;
    
//     // Use the client ID of your app for this call,
//     // same as in the configuration earlier
//     var bearerToken = adal.getCachedToken('aabbccee-aabb-1122-3344-556677889900')
   
//    var opts = {
//         method: method,
//         headers: {
//             'Authorization': 'Bearer ' +  bearerToken
//         }
//     }

//     // I'm using JSON for my API, you can change this to your
//     // heart's desire
//     if (body) {
//         opts.body = JSON.stringify(body)
//         opts.headers['Content-Type'] = 'application/json'
//     }

//     return fetch(resourceUrl, opts)
//         .then(response => {
//             return response.json()
//         }).catch(function(error) {
//             console.log("Network problem: " + error)
//             // Inspect the error further to see what is actually wrong
//         }
//     )
// }
