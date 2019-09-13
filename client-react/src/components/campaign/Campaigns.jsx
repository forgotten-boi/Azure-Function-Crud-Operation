import React, { Component } from 'react'


import CampaignItem from './CampaignItem';
import PropTypes from 'prop-types';


export class Campaigns extends Component {
    render() {
         return this.props.campaigns.map((campaign) => (
            <CampaignItem key={campaign.id} campaign={campaign} markComplete={this.props.markComplete} delCampaign={this.props.delCampaign} />
            ));
    }
}

// PropTypes
Campaigns.propTypes = {
  campaigns: PropTypes.array.isRequired,
  markComplete: PropTypes.func.isRequired,
  delCampaign: PropTypes.func.isRequired,
}


export default Campaigns
