import React, { Component } from 'react'
import PropTypes from 'prop-types';

export class CampaignItem extends Component {
  
    getStyle = () => {
    return {
      background: '#f4f4f4',
      padding: '10px',
      borderBottom: '1px #ccc dotted',
      textDecoration: this.props.campaign.completed ? 'line-through' : 'none'
    }
  }
  
  render() {
    const { id, title, description, startDate, endDate } = this.props.campaign;
    return (
      <div style={this.getStyle()}>
        <div>
          <input type="checkbox" onChange={this.props.markComplete.bind(this, id)} /> {' '}
          {title} ({startDate} - {endDate})
          <button onClick={this.props.delCampaign.bind(this, id)} style={btnStyle}>x</button>

          <div>
            { description}
          </div>
        </div>
      </div>
    )
  }
}

CampaignItem.propTypes = {
  campaign: PropTypes.object.isRequired,
  markComplete: PropTypes.func.isRequired,
  delCampaign: PropTypes.func.isRequired,
}

const btnStyle = {
  background: '#ff0000',
  color: '#fff',
  border: 'none',
  padding: '5px 9px',
  borderRadius: '50%',
  cursor: 'pointer',
  float: 'right'
}

export default CampaignItem
