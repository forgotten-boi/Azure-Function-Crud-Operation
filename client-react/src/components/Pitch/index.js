import React from 'react'

import './index.css'

const Pitch = () => {
  return (
    <div className='section-pitch'>
      <h1>Serverless Website</h1>
      <p>Azure AD Authenticated React App</p>
      <button onClick={() => {
        window.location = '/dashboard'
      }}
      >
        Sign In
      </button>
    </div>
  )
}

export default Pitch
