/* global localStorage alert */
import React, { Component } from 'react'
// import { BrowserRouter as Router, Route } from 'react-router-dom';
import { adalApiFetch, logOutUrl, withAdalLoginApi, adalUserInfo } from '../../config/adal'

import Fallback from '../../components/Fallback'
//import logo from '../../assets/images/logo.svg'
import './index.css'

import Header from '../../components/layout/Header';
import Campaigns from '../../components/campaign/Campaigns';
import AddCampaign from '../../components/campaign/AddCampaign';




import 'bootstrap/dist/css/bootstrap.min.css';

class Dashboard extends Component {

  state = {
    campaigns: [],
    user: undefined,
    token: undefined,
    updatemodel:undefined
	};

  componentDidMount () {
    this.setState({
      user: adalUserInfo(),
      // campaigns:  this.getCampaigns().data
    })

    adalApiFetch('GetAllObjects', {
      credentials: 'same-origin'
    })
    .then(res => res.json())

     .then((d) =>
      {
         this.setState({ campaigns: d})
     
      })
  


  console.log('campaignsinfo: ' + this.state.campaigns);
  console.log('token:' + this.state.token);
  
  }

  getCampaigns()
  {
     adalApiFetch('GetCollectionFromSqlServer', {
      credentials: 'same-origin'
    }).then(res => res.json())
      .then(res => {
        console.log(res)
        return res.data
      })
     
      .catch(console.error)
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




  
//Not implemented on azure function
    markComplete = (id) => {
		this.setState({
			campaigns: this.state.campaigns.map((campaign) => {
				if (campaign.id === id) {
          campaign.completed = !campaign.isComplete;
        
        }
        else 
        {
          campaign.completed = false;          
        }
				return campaign;
			})
    });
   
    const camp = [...this.state.campaigns].find((campaign) => campaign.id === id);
    // const updatemodel = this.state.updatemodel;
    // updatemodel.push(camp);

    this.setState({ updatemodel: camp });
    
    console.log(this.state.updatemodel);
	};

    	// Delete Campaign
	delCampaign = (id) => {

   const campaign = [...this.state.campaigns].find((campaign) => campaign.id === id);
   console.log(campaign);
    if(window.confirm(`Are you sure to delete this campaign? "${campaign.title}"`))
  {
    adalApiFetch(`objects/${id}`, {
      credentials: 'same-origin',
      method:'delete'
    })
    .then(res  =>
		{	
      debugger;
      	this.setState({

					campaigns: [...this.state.campaigns].filter((campaign) => campaign.id !== id)
				})
     
      }	);
    }
  };
  


	// Add Campaign
	addCampaign = (st) => {
 
    console.log(st);
  
    adalApiFetch('CreateObject', {
      credentials: 'same-origin',
      method:'post',
      body: JSON.stringify(st)
    })
    .then(res =>
       res.json())
     .then((d) =>
      {
        if(st.id === d.id)
        { 
          this.setState({

            campaigns: [...this.state.campaigns].filter((campaign) => campaign.id !== st.id)
          })
        }
          const campaigns = this.state.campaigns;
          campaigns.push(d);
          this.setState({ campaigns: campaigns });
      
      })

	};



    render() {
     
        	return (
            // <Router>
            <div className="App">
              <div className="container">
                <Header />
                <div style={{float:"right"}}>
                  <button className="button" onClick={this.handleUserDetails}>
                    User Details
                  </button>
                  <button className="button" onClick={this.handleTestApi}>
                    Test API
                  </button>
                  <button className="button" onClick={this.handleFunctionContext}>
                    Function Context
                  </button>
                  <button className="button" onClick={this.handleLogout}>
                    Log Out
                  </button>
                </div>
                <hr></hr>
                {/* <Route
							exact
							path='/'
							render={(props) => (
								<React.Fragment> */}
                <AddCampaign addCampaign={this.addCampaign} updatemodel={this.state.updatemodel} />

                <hr></hr>

                <Campaigns
                  campaigns={this.state.campaigns}
                  markComplete={this.markComplete}
                  delCampaign={this.delCampaign}
                />
                {/* </React.Fragment>
							)}
						/> */}
                {/* <Route path='/about' component={About} /> */}
              </div>
            </div>
          );
          }} 

// export default runWithAdal(authContext, () => <Dashboard />, DO_NOT_LOGIN)
export default withAdalLoginApi(Dashboard, () => <Fallback />, (err) => <Fallback error={err} />)
