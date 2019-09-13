import React, { Component } from 'react'
import PropTypes from 'prop-types'

import 'bootstrap/dist/css/bootstrap.min.css';
import { Form } from 'react-bootstrap';


export class AddCampaign extends Component {

    state = {
        id:'',
        title: '',
        description: '',
        startDate: '',
        endDate:'',
        isComplete: ''
    }

    componentWillReceiveProps(nextProps) {
        console.log(this.props);
        console.log(nextProps.updatemodel);
        if (nextProps.updatemodel) {
            const { id, title, description, startDate, endDate } = nextProps.updatemodel;
            this.setState({
                title,
                description,
                startDate,
                endDate,
                id
            });
        }
        
    }
    
    onSubmit = (e) => {
        e.preventDefault();
        this.props.addCampaign(this.state);
        this.setState({ title: '', description: '', startDate:'', endDate:'', isComplete:'',id:'' });
    }

    onChange = (e) => this.setState({ [e.target.name]: e.target.value });

    // shouldComponentUpdate(nextProps) {
    //     debugger;
    //     if(nextProps.updatemodel !== undefined && this.props.updatemodel !== nextProps.updatemodel)
    //     {
    //         const { id, title, description, startDate, endDate } = nextProps.updatemodel;
    //         console.log(title);
    //         this.setState({ title: title, description: description, startDate:startDate, endDate:endDate, isComplete:'', id:id });
    //         return true;
    //     }
    // return false;
    // }

    render() {
        console.log(this.state);

    
      
        return (
           
            <form onSubmit={this.onSubmit} >
             
                 <Form.Group controlId="title">
                    <Form.Label>Title*</Form.Label>
                    <Form.Control type="text"   value={this.state.title} name="title" required="true" placeholder="Enter Title" onChange={this.onChange}/>
                    <Form.Text className="text-muted">
                    Title of the Campaign
                    </Form.Text>
                </Form.Group>

                <Form.Group controlId="description">
                    <Form.Label>Description</Form.Label>
                    <Form.Control type="text"   value={this.state.description} name="description" placeholder="Description" onChange={this.onChange}/>
                </Form.Group>
                <Form.Group controlId="startDate">
                    <Form.Label>StartDate</Form.Label>
                    <Form.Control type="date" placeholder="StartDate" name="startDate"  value={this.state.startDate} onChange={this.onChange}/>
                </Form.Group>
                <Form.Group controlId="endDate">
                    <Form.Label>EndDate</Form.Label>
                    <Form.Control type="date" placeholder="EndDate"  name="endDate"  value={this.state.endDate} onChange={this.onChange}/>
                </Form.Group>
                {/* <Form.Group controlId="formBasicCheckbox">
                    <Form.Check type="checkbox" label="IsComplete" />
                </Form.Group> */}

                

                <input 
                type="submit" 
                value="Submit" 
                className="btn btn-primary"
                style={{flex: '1'}}
                />
            </form>
           
    )
  }
    }

//PropTypes
AddCampaign.propTypes = {
  addCampaign: PropTypes.func.isRequired,
  updatemodel: PropTypes.object
}



export default AddCampaign



