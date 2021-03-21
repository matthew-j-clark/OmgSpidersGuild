import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchData } from './components/FetchData';
import { Counter } from './components/Counter';

import './custom.css'
import { Streams } from './components/Streams';
import { ApplicationForm } from './components/ApplicationForm';

export default class App extends Component {
    static displayName = App.name;
    render() {
        return (
            <Layout>
                <Route exact path='/' component={Home} />
                <Route path='/streams' component={Streams} />
                <Route path='/ApplicationForm' component={ApplicationForm} />
            </Layout>
        );
    }
}
