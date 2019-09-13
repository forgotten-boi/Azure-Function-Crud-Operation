/* global localStorage alert */
import React, { Component } from 'react'
import { adalApiFetch, logOutUrl, withAdalLoginApi, adalUserInfo } from '../../config/adal'

import Fallback from '../../components/Fallback'
import logo from '../../assets/images/logo.svg'
import './index.css'

class Dashboard extends Component {
  constructor (props) {
    super(props)
    this.state = {
      user: undefined
    }

    this.inputElement = null
  }

  componentDidMount () {
    this.setState({
      user: adalUserInfo()
    })
  }

  async handleLogout () {
    console.log('Logging out..')
    console.log(logOutUrl)
    localStorage.clear()
    window.location.href = logOutUrl
  }

  async handleTestApi () {
    adalApiFetch('TestFunction', {
      credentials: 'same-origin'
    })
      .then(res => {
        console.log(res)
        return res.json()
      })
      .then(res => alert(res.title))
      .catch(console.error)
  }

  async handleFunctionContext () {
    adalApiFetch('CheckContext', {
      credentials: 'same-origin'
    })
      .then(res => {
        console.log(res)
        return res.json()
      })
      .then(console.log)
      .catch(console.error)
  }

  handleUserDetails () {
    console.log('User Details', adalUserInfo())
  }

  render () {
    const { user } = this.state
    return (
      <div className='App'>
        <header className='App-header'>
          <img
            className='App-logo'
            src={logo}
            alt='Logo'
          />
          <h1 className='App-title'>Welcome, {user && user.profile.name}</h1>
          <button
            className='button'
            onClick={this.handleUserDetails}
          >
            User Details
          </button>
          <button
            className='button'
            onClick={this.handleTestApi}
          >
            Test API
          </button>
          <button
            className='button'
            onClick={this.handleFunctionContext}
          >
            Function Context
          </button>
          <button
            className='button'
            onClick={this.handleLogout}
          >
            Log Out
          </button>
          <p className='App-intro'>
            To get started, edit <code>src/containers/Dashboard/index.js</code> and save to reload.
          </p>
        </header>
      </div>
    )
  }
}

// export default runWithAdal(authContext, () => <Dashboard />, DO_NOT_LOGIN)
export default withAdalLoginApi(Dashboard, () => <Fallback />, (err) => <Fallback error={err} />)
