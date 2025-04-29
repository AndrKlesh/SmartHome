import ReactRefreshWebpackPlugin from '@pmmmwh/react-refresh-webpack-plugin'
import {CleanWebpackPlugin} from 'clean-webpack-plugin'
import HtmlWebpackPlugin from 'html-webpack-plugin'
import path from 'path'

export default
	{
		mode: 'development',
		entry: './src/Main.tsx',
		output:
		{
			path: path.resolve(process.cwd(), 'dist'),
			filename: 'bundle.js',
		},
		resolve:
		{
			extensions: ['.tsx', '.ts', '.js'],
		},
		module:
		{
			rules:
				[
					{
						test: /\.(ts|tsx)$/,
						use: 'ts-loader',
						exclude: /node_modules/,
					},
					{
						test: /\.(js|jsx)$/,
						use: 'babel-loader',
						exclude: /node_modules/,
					},
					{
						test: /\.css$/,
						use: ['style-loader', 'css-loader'],
					},
					{
						test: /\.(png|jpg|gif|svg)$/,
						type: 'asset/resource',
					},
				],
		},
		plugins:
			[
				new CleanWebpackPlugin(),
				new HtmlWebpackPlugin({
					template: './index.html',
				}),
				new ReactRefreshWebpackPlugin(),
			],
		devServer:
		{
			static: path.resolve(process.cwd(), 'dist'),
			hot: true,
			historyApiFallback: true,
			port: 5173,
			open: false,
		},
	}
